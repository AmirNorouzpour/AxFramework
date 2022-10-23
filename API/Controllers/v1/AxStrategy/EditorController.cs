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
using Newtonsoft.Json;
using WebFramework.Api;
using System.Collections.Generic;
using WebFramework.Filters;
using AxIndicators.Model;
using Common.CSCodeExecuter;
using Skender.Stock.Indicators;
using DynamicExpresso;
using Common.Utilities;

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
        public async Task<ApiResult<UserStrategyDto>> Save(UserStrategyDto data, CancellationToken cancellationToken)
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

            data.Version = entity.Version;
            data.Unique = entity.Unique;
            return Ok(data);
        }
        public List<IAxIndicator> idxs = new() { new Close(), new Rsi(14) };

        [HttpPost("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public async Task<ApiResult<object>> Run(UserStrategyDto data, CancellationToken cancellationToken)
        {
            //var entity = data.ToEntity();
            var quotes = new List<Quote> { new() { Close = (decimal)1469.68 }, new() { Close = (decimal)1467.71 }, new() { Close = (decimal)1468.05 }, new() { Close = (decimal)1467.80 }, new() { Close = (decimal)1467.00 }, new() { Close = (decimal)1464.76 }, new() { Close = (decimal)1463.50 }, new() { Close = (decimal)1461.17 }, new() { Close = (decimal)1463.70 }, new() { Close = (decimal)1461.33 }, new() { Close = (decimal)1461.76 }, new() { Close = (decimal)1462.87 }, new() { Close = (decimal)1461.41 }, new() { Close = (decimal)1457.69 }, new() { Close = (decimal)1446.06 }, new() { Close = (decimal)1456.14 }, new() { Close = (decimal)1451.81 }, new() { Close = (decimal)1461.00 }, new() { Close = (decimal)1457.01 }, new() { Close = (decimal)1461.39 }, new() { Close = (decimal)1464.37 }, new() { Close = (decimal)1462.99 }, new() { Close = (decimal)1466.33 }, new() { Close = (decimal)1471.54 }, new() { Close = (decimal)1467.96 }, new() { Close = (decimal)1454.55 }, new() { Close = (decimal)1452.74 }, new() { Close = (decimal)1455.11 }, new() { Close = (decimal)1450.70 }, new() { Close = (decimal)1453.54 }, new() { Close = (decimal)1454.98 }, new() { Close = (decimal)1452.49 }, new() { Close = (decimal)1451.67 }, new() { Close = (decimal)1455.16 }, new() { Close = (decimal)1453.38 }, new() { Close = (decimal)1453.76 }, new() { Close = (decimal)1454.79 }, new() { Close = (decimal)1450.50 }, new() { Close = (decimal)1451.32 }, new() { Close = (decimal)1452.48 }, new() { Close = (decimal)1451.31 }, new() { Close = (decimal)1449.98 }, new() { Close = (decimal)1454.52 }, new() { Close = (decimal)1453.43 }, new() { Close = (decimal)1452.70 }, new() { Close = (decimal)1453.16 }, new() { Close = (decimal)1454.21 }, new() { Close = (decimal)1455.77 } };
            var res = "";
            var dbEntity = await _repository.GetAll(x => x.Unique == data.Unique).OrderByDescending(x => x.Version).FirstOrDefaultAsync(cancellationToken);
            if (dbEntity != null)
            {
                var model = JsonConvert.DeserializeObject<Model>(dbEntity.Data);
                for (var index = 0; index < model.boxs.Count; index++)
                {
                    var box = model.boxs[index];
                    if (index == 0)
                    {
                        res = @$"var res = quotes
  //.Validate()
  .Use(CandlePart.{box.indicator.title.FirstCharToUpper()})";
                        continue;
                    }

                    var ps = "";
                    var parameters = box.indicator.parameters.Where(x => x.isInput && x.title != "Source").ToList();
                    foreach (var p in parameters)
                    {
                        ps += "," + 14;
                    }

                    if (ps.Length > 0)
                        ps = ps.Remove(0, 1);
                    res += $".Get{box.indicator.title.FirstCharToUpper()}({ps})";
                }
            }

            res += "; return res;";

            var value = ScriptManager.ExecuteScript(res, quotes);

            return Ok(value);


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
                x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().Name,
                DateTime = x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().ModifiedDateTime.HasValue ? x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().ModifiedDateTime.Value : x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().InsertDateTime,
                x.OrderByDescending(y => y.InsertDateTime).FirstOrDefault().Version,
                Versions = x.OrderByDescending(y => y.Version).Select(y => y.Version)
            });
            return Ok(strategies);
        }

    }
}
