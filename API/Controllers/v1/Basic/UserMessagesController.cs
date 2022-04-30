using System.Linq;
using API.Models;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data.Repositories;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Basic
{
    [ApiVersion("1")]
    public class UserMessagesController : BaseController
    {
        private readonly IBaseRepository<UserMessage> _repository;

        public UserMessagesController(IBaseRepository<UserMessage> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public virtual ApiResult<IQueryable<UserMessageDto>> Get([FromQuery] DataRequest request)
        {
            var data = _repository.GetAll().Skip(request.PageIndex * request.PageSize).Take(request.PageSize).OrderBy(request.Sort, request.SortType).ProjectTo<UserMessageDto>();
            return Ok(data);
        }

        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public virtual ApiResult<IQueryable<UserMessageDto>> GetUserNotificationList()
        {
            var data = _repository.GetAll(x => x.Receivers.Any(r => r.PrimaryKey == UserId && !r.IsSeen)).OrderByDescending(x=>x.InsertDateTime).ProjectTo<UserMessageDto>();
            return Ok(data);
        }
    }
}