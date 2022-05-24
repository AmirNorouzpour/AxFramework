using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Hubs;
using API.Models;
using API.Models.Tracking;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Exception;
using Common.Utilities;
using Data.Repositories;
using Entities.Framework.AxCharts;
using Entities.Framework.Reports;
using Entities.Tracking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Tracking
{

    [ApiVersion("1")]
    public class ProductInstancesController : BaseController
    {
        private readonly IBaseRepository<ProductInstance> _repository;
        private readonly IBaseRepository<Personnel> _personnelRepository;
        private readonly IBaseRepository<ProductInstanceHistory> _productInstanceHistoryRepository;
        private readonly IBaseRepository<Machine> _machineRepository;
        private readonly IBaseRepository<Damaged> _damagedRepository;
        private readonly IBaseRepository<Stop> _stopRepository;
        private readonly IBaseRepository<StopDetail> _stopDetailRepository;
        private readonly IBaseRepository<Shift> _shiftRepository;
        private readonly IUserConnectionService _userConnectionService;
        private readonly IBaseRepository<BarChart> _barChartRepository;
        private readonly IHubContext<AxHub> _hub;


        public ProductInstancesController(IBaseRepository<ProductInstance> repository, IBaseRepository<Personnel> personnelRepository, IBaseRepository<ProductInstanceHistory> productInstanceHistoryRepository, IUserConnectionService userConnectionService, IBaseRepository<BarChart> barChartRepository, IHubContext<AxHub> hub, IBaseRepository<Machine> machineRepository, IBaseRepository<Damaged> damagedRepository, IBaseRepository<Stop> stopRepository, IBaseRepository<StopDetail> stopDetailRepository, IBaseRepository<Shift> shiftRepository)
        {
            _repository = repository;
            _personnelRepository = personnelRepository;
            _productInstanceHistoryRepository = productInstanceHistoryRepository;
            _userConnectionService = userConnectionService;
            _barChartRepository = barChartRepository;
            _hub = hub;
            _machineRepository = machineRepository;
            _damagedRepository = damagedRepository;
            _stopRepository = stopRepository;
            _stopDetailRepository = stopDetailRepository;
            _shiftRepository = shiftRepository;
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.ProductInstanceList)]
        public virtual ApiResult<IQueryable<ProductInstanceDto>> Get([FromQuery] DataRequest request, string code = null, string userIds = null, DateTime? date1 = null, DateTime? date2 = null)
        {
            //var predicate = request.GetFilter<ProductInstance>();
            var data0 = _repository.GetAll();
            if (!string.IsNullOrWhiteSpace(code))
                data0 = data0.Where(x => x.Code == code);

            if (userIds != null)
            {
                var userIdsArray = userIds.Split(',').Select(int.Parse);
                data0 = data0.Where(x => userIdsArray.Contains(x.Personnel.UserId));
            }

            if (date1.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date >= date1.Value.Date);

            if (date2.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date <= date2.Value.Date);

            var data = data0.OrderBy(request.Sort, request.SortType).OrderByDescending(x => x.Id).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<ProductInstanceDto>();
            Response.Headers.Add("X-Pagination", data0.Count().ToString());
            return Ok(data);
        }

        [HttpGet("{productInstanceId}")]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.ProductInstanceItem)]
        public virtual ApiResult<ProductInstanceDto> Get(int productInstanceId, int userId)
        {
            var productInstanceDto = _repository.GetAll(x => x.Id == productInstanceId).ProjectTo<ProductInstanceDto>().SingleOrDefault();
            return Ok(productInstanceDto);
        }

        [HttpPost]
        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.ProductInstanceInsert)]
        public virtual async Task<ApiResult<ProductInstanceDto>> Create(ProductInstanceDto dto, CancellationToken cancellationToken)
        {
            dto.IsActive = true;
            await _repository.AddAsync(dto.ToEntity(), cancellationToken);
            var resultDto = await _repository.TableNoTracking.ProjectTo<ProductInstanceDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
            return resultDto;
        }

        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.ProductInstanceDelete, Order = 3)]
        [HttpDelete("{id}")]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var model = await _repository.GetFirstAsync(x => x.Id.Equals(id), cancellationToken);
            await _repository.DeleteAsync(model, cancellationToken);
            return Ok();
        }

        [HttpPut]
        [AxAuthorize(StateType = StateType.Authorized, Order = 2, AxOp = AxOp.ProductInstanceUpdate)]
        public virtual async Task<ApiResult<ProductInstanceDto>> Update(ProductInstanceDto dto, CancellationToken cancellationToken)
        {
            var productInstance = await _repository.GetFirstAsync(x => x.Id == dto.Id, cancellationToken);
            if (productInstance == null)
                throw new NotFoundException("نمونه محصولی یافت نشد");

            await _repository.UpdateAsync(dto.ToEntity(productInstance), cancellationToken);
            var resultDto = await _repository.TableNoTracking.ProjectTo<ProductInstanceDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
            return resultDto;
        }

        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken, Order = 1, AxOp = AxOp.ProductInstanceInsert)]
        public virtual async Task<ApiResult<ProductInstanceDto>> AddHistory(ProductInstanceDto dto, CancellationToken cancellationToken)
        {
            var personel = await _personnelRepository.GetFirstAsync(x => x.UserId == UserId, cancellationToken);

            var machine = await _machineRepository.GetFirstAsync(x => x.Code == dto.Code, cancellationToken);
            if (machine != null)
            {
                return new ApiResult<ProductInstanceDto>(true, ApiResultStatusCode.Success, new ProductInstanceDto { MachineId = machine.Id, IsMachine = true }, " ماشین انتخاب شد");
            }

            if (dto.MachineId == 0)
            {
                return new ApiResult<ProductInstanceDto>(false, ApiResultStatusCode.LogicError, null, "ابتدا بارکد ماشین را اسکن کنید");
            }

            if (personel == null)
                return new ApiResult<ProductInstanceDto>(false, ApiResultStatusCode.LogicError, null, "برای کاربری پرسنل تعریف نشده است");

            var shifId = 0;
            var m = await _machineRepository.GetAll(x => x.Id == dto.MachineId).Include(x => x.OperationStation).FirstOrDefaultAsync(cancellationToken);
            var shifts = _shiftRepository.GetAll().ToList();
            foreach (var shift in shifts)
            {
                var ssItems = shift.StartTime.Split(':');
                var seItems = shift.StartTime.Split(':');
                var start = new TimeSpan(int.Parse(ssItems[0]), int.Parse(ssItems[1]), 0);
                var end = new TimeSpan(int.Parse(seItems[0]), int.Parse(seItems[1]), 0);
                TimeSpan now = DateTime.Now.TimeOfDay;

                if (start < end)
                    if (start <= now && now <= end)
                    {
                        shifId = shift.Id;
                    }
            }


            var p = _repository.GetFirst(x => x.Code == dto.Code);
            if (p == null)
            {
                dto.IsActive = true;
                dto.PersonnelId = personel.Id;
                dto.ProductLineId = m.OperationStation.ProductLineId;
                var productInstance = dto.ToEntity();
                await _repository.AddAsync(productInstance, cancellationToken);

                var pih = new ProductInstanceHistory
                {
                    UserId = UserId,
                    MachineId = dto.MachineId,
                    CreatorUserId = UserId,
                    InsertDateTime = DateTime.Now,
                    PersonnelId = personel.Id,
                    ShiftId = shifId,
                    ProductInstanceId = productInstance.Id,
                    EnterTime = DateTime.Now
                };
                await _productInstanceHistoryRepository.AddAsync(pih, cancellationToken);
            }
            else
            {

                var row = await _productInstanceHistoryRepository.GetFirstAsync(x => x.MachineId == dto.MachineId && x.ProductInstance.Code == dto.Code && x.EnterTime != null, cancellationToken);
                //var hasExit = await _productInstanceHistoryRepository.GetFirstAsync(x => x.MachineId == dto.MachineId && x.ProductInstance.Code == dto.Code && x.ExitTime != null, cancellationToken);

                if (row != null && dto.IsEnter)
                    return new ApiResult<ProductInstanceDto>(false, ApiResultStatusCode.LogicError, null, "ورود این قطعه قبلا در ایستگاه ثبت شده است");
                //if (hasExit != null && !dto.IsEnter)
                //    return new ApiResult<ProductInstanceDto>(false, ApiResultStatusCode.LogicError, null, "خروج این قطعه از ایستگاه قبلا ثبت شده است");

                if (!dto.IsEnter && row == null)
                {
                    return new ApiResult<ProductInstanceDto>(false, ApiResultStatusCode.LogicError, null, "این قطعه هنوز وارد ایستگاه نشده است");
                }

                var pih = new ProductInstanceHistory
                {
                    UserId = UserId,
                    MachineId = dto.MachineId,
                    CreatorUserId = UserId,
                    InsertDateTime = DateTime.Now,
                    PersonnelId = personel.Id,
                    ShiftId = shifId,
                    ProductInstanceId = p.Id
                };

                if (dto.IsEnter)
                    pih.EnterTime = DateTime.Now;

                if (!dto.IsEnter && row != null)
                {
                    row.ExitTime = DateTime.Now;
                    await _productInstanceHistoryRepository.UpdateAsync(row, cancellationToken);
                }
                else
                {
                    await _productInstanceHistoryRepository.AddAsync(pih, cancellationToken);
                }
            }

            var connections = _userConnectionService.GetActiveConnections();
            var barChart = _barChartRepository.GetAll(x => x.AxChartId == 17).ProjectTo<BarChartDto>().FirstOrDefault();
            if (barChart != null)
            {

                var pid = 2;
                var data = _productInstanceHistoryRepository
                    .GetAll(x => !x.ExitTime.HasValue && x.Machine.OperationStation.ProductLineId == pid)
                    .Include(x => x.Machine).ThenInclude(x => x.OperationStation).ToList().GroupBy(x => x.Machine.OperationStationId)
                    .Select(x => new { Count = x.Count(), x.Key, Data = x })
                    .ToList();
                var a = data.OrderBy(x => x.Data.FirstOrDefault()?.Machine.OperationStation.Order).Select(x => x.Count)
                    .ToList();
                barChart.Series.Add(new AxSeriesDto { Data = a, Name = "تعداد محصول" });
                barChart.Labels = data.Select(x => x.Data.FirstOrDefault()?.Machine.OperationStation.Name).ToList();
            }

            await _hub.Clients.Clients(connections).SendAsync("UpdateChart", barChart, cancellationToken);

            var resultDto = await _repository.TableNoTracking.ProjectTo<ProductInstanceDto>().SingleOrDefaultAsync(instanceDto => instanceDto.Id.Equals(dto.Id), cancellationToken);
            return resultDto;
        }


        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken, Order = 1, AxOp = AxOp.ProductInstanceInsert)]
        public virtual async Task<ApiResult<Damaged>> AddDamaged(ProductInstanceDto dto, CancellationToken cancellationToken)
        {
            var personel = await _personnelRepository.GetFirstAsync(x => x.UserId == UserId, cancellationToken);

            if (personel == null)
                return new ApiResult<Damaged>(false, ApiResultStatusCode.LogicError, null, "برای کاربری پرسنل تعریف نشده است");


            var p = await _repository.GetFirstAsync(x => x.Code == dto.Code, cancellationToken);
            if (p == null)
            {
                return new ApiResult<Damaged>(false, ApiResultStatusCode.LogicError, null, "این محصول هنوز وارد چرخه تولید نشده است");
            }

            var damaged = new Damaged
            {
                CreatorUserId = UserId,
                InsertDateTime = DateTime.Now,
                PersonnelId = personel.Id,
                DateTime = DateTime.Now,
                ProductInstanceId = p.Id,
                DamageCode = dto.UserName
            };

            var damagedOld = await _damagedRepository.GetFirstAsync(x => x.ProductInstanceId == p.Id, cancellationToken);
            if (damagedOld != null)
                return new ApiResult<Damaged>(false, ApiResultStatusCode.LogicError, null, "این محصول قبلا بعنوان ضایعات ثبت شده است");

            await _damagedRepository.AddAsync(damaged, cancellationToken);

            return damaged;
        }


        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.ProductInstanceList)]
        public virtual ApiResult<IQueryable<DamagedDto>> GetDamagedList([FromQuery] DataRequest request, string code = null, string userIds = null, DateTime? date = null)
        {
            //var predicate = request.GetFilter<ProductInstance>();
            var data0 = _damagedRepository.GetAll().Include(x => x.ProductInstance).Include(x => x.Personnel.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(code))
                data0 = data0.Where(x => x.ProductInstance.Code == code);

            if (userIds != null)
            {
                var userIdsArray = userIds.Split(',').Select(int.Parse);
                data0 = data0.Where(x => userIdsArray.Contains(x.Personnel.UserId));
            }

            if (date.HasValue)
                data0 = data0.Where(x => x.DateTime.Date == date.Value.Date);

            var data = data0.OrderBy(request.Sort, request.SortType).OrderByDescending(x => x.Id).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<DamagedDto>();
            Response.Headers.Add("X-Pagination", data0.Count().ToString());
            return Ok(data);
        }

        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.ProductInstanceList)]
        public virtual ApiResult<List<StopDto>> GetStopList([FromQuery] DataRequest request, string code = null, int? machineId = null, DateTime? date = null)
        {
            //var predicate = request.GetFilter<ProductInstance>();
            var data0 = _stopRepository.GetAll().Include(x => x.Machine).Include(x => x.StopDetails).AsQueryable();
            if (!string.IsNullOrWhiteSpace(code))
                data0 = data0.Where(x => x.Code == code);

            if (machineId.HasValue)
                data0 = data0.Where(x => x.MachineId == machineId.Value);

            if (date.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date == date.Value.Date);

            var data = data0.OrderBy(request.Sort, request.SortType).OrderByDescending(x => x.Id)
                .Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ToList()
                .Select(x => new StopDto
                {
                    Id = x.Id,
                    MachineCode = x.Machine.Code,
                    InsertDateTime = x.InsertDateTime,
                    LastStatus = x.StopDetails.OrderByDescending(x => x.InsertDateTime).FirstOrDefault()!.StopDetailType.ToString(),
                    Code = x.Code,
                    MachineName = x.Machine.Name
                }).ToList();
            Response.Headers.Add("X-Pagination", data.Count().ToString());
            return Ok(data);
        }

        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken, Order = 1, AxOp = AxOp.ProductInstanceInsert)]
        public virtual async Task<ApiResult<StopDto>> AddStop(StopDto dto, CancellationToken cancellationToken)
        {
            var personel = await _personnelRepository.GetFirstAsync(x => x.UserId == UserId, cancellationToken);

            if (personel == null)
                return new ApiResult<StopDto>(false, ApiResultStatusCode.LogicError, null, "برای کاربری پرسنل تعریف نشده است");

            var machine = await _machineRepository.GetFirstAsync(x => x.Code == dto.MachineCode, cancellationToken);
            if (machine == null)
            {
                return new ApiResult<StopDto>(false, ApiResultStatusCode.LogicError, null, "ماشین یافت نشد");
            }

            if (dto.Step == 1)
            {
                var stop = new Stop
                {
                    CreatorUserId = UserId,
                    InsertDateTime = DateTime.Now,
                    MachineId = machine.Id,
                    Code = dto.Code
                };
                await _stopRepository.AddAsync(stop, cancellationToken);

                var stopDetail = new StopDetail
                {
                    CreatorUserId = UserId,
                    InsertDateTime = DateTime.Now,
                    StopDetailType = (StopDetailType)dto.Step,
                    PersonnelId = personel.Id,
                    StopId = stop.Id
                };
                await _stopDetailRepository.AddAsync(stopDetail, cancellationToken);
            }
            else
            {
                var stopOld = await _stopRepository.GetAll(x => x.Code == dto.Code && x.Machine.Code == dto.MachineCode).Include(x => x.Machine).OrderByDescending(x => x.InsertDateTime).FirstOrDefaultAsync(cancellationToken);
                if (stopOld == null)
                    return new ApiResult<StopDto>(false, ApiResultStatusCode.LogicError, dto, "Old Stop is null!");
                var stopDetail = new StopDetail
                {
                    CreatorUserId = UserId,
                    InsertDateTime = DateTime.Now,
                    StopDetailType = (StopDetailType)dto.Step,
                    PersonnelId = personel.Id,
                    StopId = stopOld.Id
                };
                await _stopDetailRepository.AddAsync(stopDetail, cancellationToken);
            }

            return new ApiResult<StopDto>(true, ApiResultStatusCode.Success, null);
        }
    }
}
