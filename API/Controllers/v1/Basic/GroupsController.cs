using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Exception;
using Data.Repositories;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;
using WebFramework.Filters;
using Common.Utilities;

namespace API.Controllers.v1.Basic
{
    /// <summary>
    /// Group Methods
    /// </summary>
    [ApiVersion("1")]
    public class GroupsController : BaseController
    {
        private readonly IBaseRepository<AxGroup> _groupRepository;

        public GroupsController(IBaseRepository<AxGroup> groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.Authorized, Order = 0, AxOp = AxOp.GroupList, ShowInMenu = true)]
        public ApiResult<IQueryable<AxGroupDto>> Get([FromQuery] DataRequest request, CancellationToken cancellationToken)
        {
            var groups = _groupRepository.GetAll().OrderBy(request.Sort, request.SortType).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<AxGroupDto>();
            Response.Headers.Add("X-Pagination", _groupRepository.Count().ToString());
            return Ok(groups);
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.Ignore, Order = 1, AxOp = AxOp.GroupItem)]
        [Route("{id}")]
        public async Task<ApiResult<AxGroupDto>> Get(int id, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetAll(x => x.Id == id).ProjectTo<AxGroupDto>().FirstOrDefaultAsync(cancellationToken);
            return Ok(group);
        }

        [HttpPost]
        [AxAuthorize(StateType = StateType.Authorized, Order = 2, AxOp = AxOp.GroupInsert)]
        public virtual async Task<ApiResult<AxGroupDto>> Create(AxGroupDto dto, CancellationToken cancellationToken)
        {
            await _groupRepository.AddAsync(dto.ToEntity(), cancellationToken);
            var resultDto = await _groupRepository.TableNoTracking.ProjectTo<AxGroupDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
            return resultDto;
        }
        [HttpPut]
        [AxAuthorize(StateType = StateType.Authorized, Order = 3, AxOp = AxOp.GroupUpdate)]
        public virtual async Task<ApiResult<AxGroupDto>> Update(AxGroupDto dto, CancellationToken cancellationToken)
        {
            var op = await _groupRepository.GetFirstAsync(x => x.Id == dto.Id, cancellationToken);
            if (op == null)
                throw new NotFoundException("پرسنلی یافت نشد");

            await _groupRepository.UpdateAsync(dto.ToEntity(op), cancellationToken);
            var resultDto = await _groupRepository.TableNoTracking.ProjectTo<AxGroupDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);
            return resultDto;
        }

        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.GroupDelete, Order = 4)]
        [HttpDelete("{id}")]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var model = await _groupRepository.GetFirstAsync(x => x.Id.Equals(id), cancellationToken);
            await _groupRepository.DeleteAsync(model, cancellationToken);
            return Ok();
        }

    }
}