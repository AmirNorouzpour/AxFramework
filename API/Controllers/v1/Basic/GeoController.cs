//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using API.Models;
//using AutoMapper.QueryableExtensions;
//using Common;
//using Common.Exception;
//using Common.Utilities;
//using Data.Repositories;
//using Entities.Framework;
//using Entities.Framework.Reports;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using WebFramework.Api;
//using WebFramework.Filters;

//namespace API.Controllers.v1.Basic
//{
//    [ApiVersion("1")]
//    public class GeoController : BaseController
//    {
//        private readonly IBaseRepository<Geo> _repository;

//        public GeoController(IBaseRepository<Geo> repository)
//        {
//            _repository = repository;
//        }

//        [HttpGet]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.GeoInfo, ShowInMenu = true)]
//        public virtual ApiResult<IQueryable<GeoDto>> Get([FromQuery] DataRequest request)
//        {
//            var predicate = request.GetFilter<Geo>();
//            var data = _repository.GetAll(predicate).OrderBy(request.Sort, request.SortType).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<GeoDto>();
//            return Ok(data);
//        }


//        [HttpGet]
//        [Route("{id}")]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.GeoInfo)]
//        public ApiResult<GeoDto> Get(int id)
//        {
//            var geo = _repository.GetAll(x => x.Id == id).ProjectTo<GeoDto>().FirstOrDefault();
//            return Ok(geo);
//        }

//        [HttpGet]
//        [Route("[action]/{parentId?}")]
//        [AxAuthorize(StateType = StateType.Ignore, Order = 1, AxOp = AxOp.GeoInfo)]
//        public ApiResult<IQueryable<Geo>> GetWithChildren(int? parentId)
//        {

//            var geos = _repository.GetAll(x => x.ParentId == parentId).Include(x => x.Children)
//                .ThenInclude(c => c.Children)
//                .ThenInclude(c => c.Children);
//            return Ok(geos);
//        }

//        [HttpPost]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.GeoInfoInsert)]
//        public virtual async Task<ApiResult<AddressDto>> Create(GeoDto dto, CancellationToken cancellationToken)
//        {
//            await _repository.AddAsync(dto.ToEntity(), cancellationToken);
//            var resultDto = await _repository.TableNoTracking.ProjectTo<AddressDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
//            return resultDto;
//        }

//        [HttpPut]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 2, AxOp = AxOp.GeoInfoUpdate)]
//        public virtual async Task<ApiResult<GeoDto>> Update(GeoDto dto, CancellationToken cancellationToken)
//        {
//            var data = await _repository.GetFirstAsync(x => x.Id == dto.Id, cancellationToken);
//            if (data == null)
//                throw new NotFoundException("موقعیتی یافت نشد");

//            await _repository.UpdateAsync(dto.ToEntity(), cancellationToken);
//            var resultDto = await _repository.TableNoTracking.ProjectTo<GeoDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
//            return resultDto;
//        }

//        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.GeoInfoDelete, Order = 3)]
//        [HttpDelete("{id}")]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.UserDelete)]
//        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
//        {
//            var model = await _repository.GetFirstAsync(x => x.Id.Equals(id), cancellationToken);
//            await _repository.DeleteAsync(model, cancellationToken);
//            return Ok();
//        }

//    }
//}
