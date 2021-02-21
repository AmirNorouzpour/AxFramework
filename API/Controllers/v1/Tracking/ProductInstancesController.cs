﻿using System;
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
        private readonly IUserConnectionService _userConnectionService;
        private readonly IBaseRepository<BarChart> _barChartRepository;
        private readonly IHubContext<AxHub> _hub;
        

        public ProductInstancesController(IBaseRepository<ProductInstance> repository, IBaseRepository<Personnel> personnelRepository, IBaseRepository<ProductInstanceHistory> productInstanceHistoryRepository, IUserConnectionService userConnectionService, IBaseRepository<BarChart> barChartRepository, IHubContext<AxHub> hub)
        {
            _repository = repository;
            _personnelRepository = personnelRepository;
            _productInstanceHistoryRepository = productInstanceHistoryRepository;
            _userConnectionService = userConnectionService;
            _barChartRepository = barChartRepository;
            _hub = hub;
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.ProductInstanceList)]
        public virtual ApiResult<IQueryable<ProductInstanceDto>> Get([FromQuery] DataRequest request)
        {
            var predicate = request.GetFilter<ProductInstance>();
            var data = _repository.GetAll(predicate).OrderBy(request.Sort, request.SortType).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<ProductInstanceDto>();
            Response.Headers.Add("X-Pagination", _repository.Count(predicate).ToString());
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
            var personel = _personnelRepository.GetFirst(x => x.UserId == UserId);

            if (personel == null)
                return new ApiResult<ProductInstanceDto>(false, ApiResultStatusCode.LogicError, null, "برای کاربری پرسنل تعریف نشده است");
            var p = _repository.GetFirst(x => x.Code == dto.Code);
            if (p == null)
            {
                dto.IsActive = true;
                dto.PersonnelId = personel.Id;
                dto.ProductLineId = 2;
                var productInstance = dto.ToEntity();
                await _repository.AddAsync(productInstance, cancellationToken);

                var pih = new ProductInstanceHistory
                {
                    UserId = UserId,
                    OpId = dto.OpId,
                    CreatorUserId = UserId,
                    InsertDateTime = DateTime.Now,
                    PersonnelId = personel.Id,
                    ShiftId = 3,
                    ProductInstanceId = productInstance.Id,
                    EnterTime = DateTime.Now
                };
                await _productInstanceHistoryRepository.AddAsync(pih, cancellationToken);
            }
            else
            {

                var row = _productInstanceHistoryRepository.GetFirst(x => x.OpId == dto.OpId && x.EnterTime != null);
                var hasExit = _productInstanceHistoryRepository.GetAll(x => x.OpId == dto.OpId && x.ExitTime != null).Any();

                if (row != null && dto.IsEnter)
                    return new ApiResult<ProductInstanceDto>(false, ApiResultStatusCode.LogicError, null, "ورود این قطعه قبلا در ایستگاه ثبت شده است");
                if (hasExit && !dto.IsEnter)
                    return new ApiResult<ProductInstanceDto>(false, ApiResultStatusCode.LogicError, null, "خروج این قطعه از ایستگاه قبلا ثبت شده است");

                var pih = new ProductInstanceHistory
                {
                    UserId = UserId,
                    OpId = dto.OpId,
                    CreatorUserId = UserId,
                    InsertDateTime = DateTime.Now,
                    PersonnelId = personel.Id,
                    ShiftId = 3,
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
                    .GetAll(x => !x.ExitTime.HasValue && x.OperationStation.ProductLineId == pid)
                    .Include(x => x.OperationStation).ToList().GroupBy(x => x.OpId)
                    .Select(x => new { Count = x.Count(), x.Key, Data = x })
                    .ToList();
                var a = data.OrderBy(x => x.Data.FirstOrDefault()?.OperationStation.Order).Select(x => x.Count)
                    .ToList();
                barChart.Series.Add(new AxSeriesDto { Data = a, Name = "تعداد محصول" });
                barChart.Labels = data.Select(x => x.Data.FirstOrDefault()?.OperationStation.Name).ToList();
            }

            await _hub.Clients.Clients(connections).SendAsync("UpdateChart", barChart, cancellationToken);

            var resultDto = await _repository.TableNoTracking.ProjectTo<ProductInstanceDto>().SingleOrDefaultAsync(instanceDto => instanceDto.Id.Equals(dto.Id), cancellationToken);
            return resultDto;
        }

    }
}
