using System;
using System.Collections.Generic;
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
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserGroup> _groupUsersRepository;

        public GroupsController(IBaseRepository<AxGroup> groupRepository, IBaseRepository<User> userRepository, IBaseRepository<UserGroup> groupUsersRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _groupUsersRepository = groupUsersRepository;
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.Authorized, Order = 0, AxOp = AxOp.GroupList, ShowInMenu = true)]
        public ApiResult<IQueryable<AxGroupDto>> Get([FromQuery] DataRequest request, CancellationToken cancellationToken)
        {
            var groups = _groupRepository.GetAll().Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<AxGroupDto>();
            Response.Headers.Add("X-Pagination", _groupRepository.Count().ToString());
            return Ok(groups);
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.Ignore, Order = 1, AxOp = AxOp.GroupItem)]
        [Route("[action]/{id}")]
        public async Task<ApiResult<List<UserGroupDto>>> GetGroupUsers(int id, CancellationToken cancellationToken)
        {
            var list = new List<UserGroupDto>();
            var group = await _groupRepository.GetFirstAsync(x => x.Id == id, cancellationToken);
            if (group != null)
            {
                await _groupRepository.LoadCollectionAsync(group, t => t.GroupUsers, cancellationToken);
                foreach (var item in group.GroupUsers)
                {
                    var user = _userRepository.GetById(item.UserId);
                    list.Add(new UserGroupDto { Name = user?.FullName, InsertDateTime = item.InsertDateTime, Id = item.Id });
                }
                return list;
            }

            return list;
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

        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.Authorized, Order = 2, AxOp = AxOp.GroupUpdate)]
        public async Task<ApiResult> AddUsers(AxGroupUsersDto dto, CancellationToken cancellationToken)
        {
            if (dto.UserIds?.Count > 0)
                foreach (var item in dto.UserIds)
                {
                    var row = await _groupUsersRepository.GetFirstAsync(x => x.GroupId == dto.GroupId && x.UserId == item, cancellationToken);
                    if (row == null)
                    {
                        await _groupUsersRepository.AddAsync(new UserGroup
                        {
                            CreatorUserId = UserId,
                            GroupId = dto.GroupId,
                            UserId = item,
                            InsertDateTime = DateTime.Now
                        }, cancellationToken);
                    }
                }

            return new ApiResult(true, ApiResultStatusCode.Success, "کاربران ثبت شدند");
        }

        [HttpDelete("[action]/{id}")]
        [AxAuthorize(StateType = StateType.Authorized, Order = 2, AxOp = AxOp.GroupUpdate)]
        public async Task<ApiResult> RemoveUser(int id, CancellationToken cancellationToken)
        {
            var row = await _groupUsersRepository.GetFirstAsync(x => x.Id == id, cancellationToken);
            if (row != null)
            {
                await _groupUsersRepository.DeleteAsync(row, cancellationToken);
                return new ApiResult(true, ApiResultStatusCode.Success, "کاربر حذف شد");
            }
            return new ApiResult(false, ApiResultStatusCode.BadRequest, "کاربر در گروه یافت نشد");
        }
    }
}