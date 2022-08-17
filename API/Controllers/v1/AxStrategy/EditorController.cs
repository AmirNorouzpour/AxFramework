using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.AxStrategy
{
    [ApiVersion("1")]
    public class EditorController : BaseController
    {
        private readonly IBaseRepository<UserStrategy> _repository;

        public EditorController(IBaseRepository<UserStrategy> repository)
        {
            _repository = repository;
        }

        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public async Task<ApiResult<string>> Save(UserStrategyDto data, CancellationToken cancellationToken)
        {
            var entity = data.ToEntity();
            var dbEntity = await _repository.GetAll(x => x.UserId == UserId && x.Unique == data.Unique).OrderByDescending(x => x.Version).FirstOrDefaultAsync(cancellationToken);
            if (dbEntity == null)
            {
                entity.UserId = UserId;
                entity.Version = 1;
                entity.Unique = Guid.NewGuid().ToString();
                await _repository.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.Version = dbEntity.Version + 1;
                entity.UserId = UserId;
                await _repository.AddAsync(entity, cancellationToken);
            }
            return Ok(entity.Unique);
        }

        [HttpGet]
        [Route("[action]/{unique}/{v}")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public async Task<ApiResult<UserStrategyDto>> GetByUnique(string unique, int v, CancellationToken cancellationToken)
        {
            var strategyDto = await _repository.GetAll(x => x.Unique == unique && x.UserId == UserId && x.Version == v).OrderByDescending(x => x.Version).ProjectTo<UserStrategyDto>().FirstOrDefaultAsync(cancellationToken);
            return strategyDto;
        }

        [HttpGet]
        [Route("[action]")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public ApiResult<IQueryable<dynamic>> GetUserStrategies()
        {
            var data = _repository.GetAll(x => x.UserId == UserId).GroupBy(x => x.Unique).OrderByDescending(x => x.FirstOrDefault().InsertDateTime);
            var strategies = data.Select(x => new
            {
                Unique = x.Key,
                x.FirstOrDefault().Name,
                DateTime = x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().ModifiedDateTime.HasValue ? x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().ModifiedDateTime.Value : x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().InsertDateTime,
                x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().Version,
                Versions = x.OrderByDescending(y => y.Version).Select(y => y.Version)
            });
            return Ok(strategies);
        }

    }
}
