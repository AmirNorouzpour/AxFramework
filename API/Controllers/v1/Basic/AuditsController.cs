//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using API.Models;
//using AutoMapper.QueryableExtensions;
//using Common;
//using Data.Repositories;
//using Entities.Framework;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using WebFramework.Api;
//using WebFramework.Filters;
//using Common.Utilities;
//using Entities.Framework.Reports;

//namespace API.Controllers.v1.Basic
//{
//    [ApiVersion("1")]
//    public class AuditsController : BaseController
//    {
//        private readonly IBaseRepository<Audit> _repository;
//        private readonly IBaseRepository<Log> _logRepository;

//        public AuditsController(IBaseRepository<Audit> repository, IBaseRepository<Log> logRepository)
//        {
//            _repository = repository;
//            _logRepository = logRepository;
//        }

//        [HttpGet]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 1, AxOp = AxOp.ChangeTracker, ShowInMenu = true)]
//        public virtual ApiResult<IQueryable<AuditDto>> Get([FromQuery] DataRequest request)
//        {
//            var predicate = request.GetFilter<Audit>();
//            var data = _repository.GetAll(predicate).OrderBy(request.Sort, request.SortType).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<AuditDto>();
//            return Ok(data);
//        }

//        [HttpGet("[action]")]
//        [AxAuthorize(StateType = StateType.Authorized, Order = 0, AxOp = AxOp.LogList, ShowInMenu = true)]
//        public virtual ApiResult<IQueryable<LogDto>> GetLogs([FromQuery] DataRequest request)
//        {
//            var predicate = request.GetFilter<Log>();
//            var data = _logRepository.GetAll(predicate).OrderBy(request.Sort, request.SortType).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<LogDto>();
//            Response.Headers.Add("X-Pagination", _logRepository.Count(predicate).ToString());
//            return Ok(data);
//        }

//        [HttpGet("[action]/{id}")]
//        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.LogItem)]
//        public ApiResult<LogDto> GetLog(int id)
//        {
//            var log = _logRepository.GetAll(x => x.Id == id).FirstOrDefault();
//            return Ok(log);
//        }


//        [HttpPost("[action]")]
//        [AxAuthorize(StateType = StateType.OnlyToken)]
//        public virtual async Task<ApiResult<LogDto>> CreateLog(Log dto, CancellationToken cancellationToken)
//        {
//            await _logRepository.AddAsync(dto, cancellationToken);
//            var resultDto = await _logRepository.TableNoTracking.ProjectTo<LogDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
//            return resultDto;
//        }
//    }
//}