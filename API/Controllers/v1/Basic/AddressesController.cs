//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using API.Models;
//using Common;
//using Common.Exception;
//using Entities.Framework;
//using Entities.Framework.Reports;
//using Microsoft.AspNetCore.Mvc;
//using WebFramework.Api;
//using WebFramework.Filters;

//namespace API.Controllers.v1.Basic
//{
//    [ApiVersion("1")]
//    public class AddressesController : BaseController
//    {
//        private readonly IBaseRepository<Address> _repository;

//        public AddressesController(IBaseRepository<Address> repository)
//        {
//            _repository = repository;
//        }

//        [HttpGet]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.AddressList)]
//        public virtual ApiResult<IQueryable<AddressDto>> Get([FromQuery] DataRequest request, int userId)
//        {
//            var predicate = request.GetFilter<Address>();
//            var data = _repository.GetAll(predicate).Where(x => x.UserId == userId).OrderBy(request.Sort, request.SortType).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<AddressDto>();
//            return Ok(data);
//        }

//        [HttpGet("{addressId}/{userId}")]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 5, AxOp = AxOp.AddressItem)]
//        public virtual ApiResult<AddressDto> Get(int addressId, int userId)
//        {
//            var address = _repository.GetAll(x => x.Id == addressId && x.UserId == userId).ProjectTo<AddressDto>().SingleOrDefault();
//            return Ok(address);
//        }

//        [HttpPost]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.AddressInsert)]
//        public virtual async Task<ApiResult<AddressDto>> Create(AddressDto dto, CancellationToken cancellationToken)
//        {
//            await _repository.AddAsync(dto.ToEntity(), cancellationToken);
//            var resultDto = await _repository.TableNoTracking.ProjectTo<AddressDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
//            return resultDto;
//        }

//        [HttpPut]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 2, AxOp = AxOp.AddressUpdate)]
//        public virtual async Task<ApiResult<AddressDto>> Update(AddressDto dto, CancellationToken cancellationToken)
//        {
//            var address = await _repository.GetFirstAsync(x => x.Id == dto.Id, cancellationToken);
//            if (address == null)
//                throw new NotFoundException("آدرسی یافت نشد");

//            await _repository.UpdateAsync(dto.ToEntity(address), cancellationToken);
//            var resultDto = await _repository.TableNoTracking.ProjectTo<AddressDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
//            return resultDto;
//        }

//        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.AddressDelete, Order = 3)]
//        [HttpDelete("{id}")]
//        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
//        {
//            var model = await _repository.GetFirstAsync(x => x.Id.Equals(id), cancellationToken);
//            await _repository.DeleteAsync(model, cancellationToken);
//            return Ok();
//        }
//    }
//}