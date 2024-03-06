using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Hubs;
using API.Models;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data.Repositories;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Basic
{
    [ApiVersion("1")]
    public class ChatController : BaseController
    {
        private readonly IBaseRepository<Message> _repository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IUserConnectionService _userConnectionService;
        private readonly IHubContext<AxHub> _hub;

        public ChatController(IBaseRepository<Message> repository, IBaseRepository<User> userRepository, IUserConnectionService userConnectionService, IHubContext<AxHub> hub)
        {
            _repository = repository;
            _userRepository = userRepository;
            _userConnectionService = userConnectionService;
            _hub = hub;
        }

        [HttpGet("[action]/{friendId}")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public virtual ApiResult<IQueryable<MessageDto>> GetMessages(int friendId)
        {
            var userId = User.Identity.GetUserId<int>();
            var data = _repository.GetAll(x => (x.SenderId == userId && x.ReceiverId == friendId) || (x.SenderId == friendId && x.ReceiverId == userId)).OrderBy(x => x.InsertDateTime).Take(1000).Select(x => new
                 MessageDto
            {
                ReceiverId = x.ReceiverId,
                Id = x.Id,
                SenderId = x.SenderId,
                Body = x.Body,
                InsertDateTime = x.InsertDateTime,
                FromContact = x.ReceiverId == userId && x.SenderId != userId
            });
            return Ok(data);
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public virtual ApiResult<IQueryable<UserDto>> GetUsers()
        {
            var data = _userRepository.GetAll(x => x.IsActive).ProjectTo<UserDto>();
            return Ok(data);
        }


        [HttpPost]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public virtual async Task<ApiResult<MessageDto>> Create(MessageDto dto, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId<int>();
            dto.SenderId = userId;
            await _repository.AddAsync(dto.ToEntity(), cancellationToken);
            var resultDto = await _repository.TableNoTracking.ProjectTo<MessageDto>().SingleOrDefaultAsync(p => p.Id.Equals(dto.Id), cancellationToken);

            dto.InsertDateTime = DateTime.Now;
            if (dto.SenderId != dto.ReceiverId)
            {
                var connections = _userConnectionService.GetActiveConnections(dto.ReceiverId);
                await _hub.Clients.Clients(connections).SendAsync("UpdateChat", dto, cancellationToken);
            }
            return resultDto;
        }
    }
}
