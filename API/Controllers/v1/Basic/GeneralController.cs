﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using Binance.Net.Clients;
using Binance.Net.Objects.Models.Futures;
using Common;
using Common.Utilities;
using CryptoExchange.Net.Authentication;
using Data.Repositories;
using Entities.Framework;
using Entities.MasterSignal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Basic
{
    [ApiVersion("1")]
    public class GeneralController : BaseController
    {
        private readonly IBaseRepository<ConfigData> _repository;
        private readonly IBaseRepository<UserData> _userDataRepository;
        private readonly IBaseRepository<AxPositionLog> _axPositionLogRepository;

        public GeneralController(IBaseRepository<ConfigData> repository, IBaseRepository<UserData> userDataRepository, IBaseRepository<AxPositionLog> axPositionLogRepository)
        {
            _repository = repository;
            _userDataRepository = userDataRepository;
            _axPositionLogRepository = axPositionLogRepository;
        }


        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public async Task<IActionResult> GetOrganizationLogo(CancellationToken cancellationToken)
        {
            var data = await _repository.GetFirstAsync(x => x.Active, cancellationToken);

            if (data == null)
                return NotFound();

            return File(data.OrganizationLogo.ToArray(), GeneralUtils.GetContentType("a.png"));
        }

        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpPost("[action]")]
        public async Task<ApiResult<UserData>> AddApiUser(UserDataDto userDataDto, CancellationToken cancellationToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress;
            //if (ip.ToString() != "2.186.122.127" || ip.ToString() != "65.108.14.168")
            //    return new ApiResult<UserData>(false, ApiResultStatusCode.UnAuthorized, null, "UnAuthorized IP");

            var userData = await _userDataRepository.GetFirstAsync(x => x.MobileNumber == userDataDto.MobileNumber, cancellationToken);
            if (userData != null)
                return new ApiResult<UserData>(true, ApiResultStatusCode.Success, userData, "اکانت قبلا ثبت شده است");

            var client = new BinanceClient();
            client.SetApiCredentials(new ApiCredentials(userDataDto.ApiKey, userDataDto.SecretKey));

            var res = await client.UsdFuturesApi.Account.GetBalancesAsync(ct: cancellationToken);
            if (res.Error?.Message != null)
                return new ApiResult<UserData>(true, ApiResultStatusCode.LogicError, null, res.Error.Message);
            var balance = res.Data.FirstOrDefault(x => x.Asset == @"USDT")?.WalletBalance;

            if (balance == null)
                return new ApiResult<UserData>(true, ApiResultStatusCode.LogicError, null, "موجودی به تتر در اکنت فیوچرز یافت نشد");


            var u = new UserData
            {
                ExpireDate = DateTime.Now.AddDays(30),
                ApiKey = userDataDto.ApiKey,
                MobileNumber = userDataDto.MobileNumber,
                //PhrasePassword = userDataDto.PhrasePassword,
                SecretKey = userDataDto.SecretKey,
                InitBalance = balance.Value,
                Balance = balance.Value,
                IsActive = true,
                IsAdmin = false
            };
            await _userDataRepository.AddAsync(u, cancellationToken);
            return u;

        }

        [AxAuthorize(StateType = StateType.Ignore)]
        [HttpPost("[action]")]
        public async Task<ApiResult<PositionModel>> GetInfo(UserDataDto userDataDto, CancellationToken cancellationToken)
        {
            var userData = await _userDataRepository.GetFirstAsync(x => x.MobileNumber == userDataDto.MobileNumber, cancellationToken);
            if (userData == null)
                return new ApiResult<PositionModel>(false, ApiResultStatusCode.LogicError, null, "Account Not Found!");

            var client = new BinanceClient();
            client.SetApiCredentials(new ApiCredentials(userData.ApiKey, userData.SecretKey));

            //var res0 = await client.UsdFuturesApi.Account.GetBalancesAsync(ct: cancellationToken);
            //if (res0.Error?.Message != null)
            //    return new ApiResult<PositionModel>(true, ApiResultStatusCode.LogicError, null, res0.Error.Message);
            //var balance = res0.Data.FirstOrDefault(x => x.Asset == @"USDT")?.WalletBalance;

            var result = new PositionModel();
            var res = await client.UsdFuturesApi.Account.GetAccountInfoAsync(ct: cancellationToken);
            if (!res.Success && res.Error != null)
                return new ApiResult<PositionModel>(false, ApiResultStatusCode.LogicError, null, res.Error.Message);

            var prices0 = await client.UsdFuturesApi.ExchangeData.GetPricesAsync(cancellationToken);
            var prices = prices0.Data.ToList();
            var list = res.Data.Positions.Where(x => x.EntryPrice != 0).ToList();

            foreach (var item in list)
            {
                item.MaintMargin = prices.FirstOrDefault(x => x.Symbol == item.Symbol)!.Price;
            }
            result.List = list;
            result.TotalMargin = res.Data.TotalMarginBalance;
            result.TotalWallet = res.Data.TotalWalletBalance;
            result.TotalUnPnl = res.Data.TotalUnrealizedProfit;
            result.MobileNumber = userDataDto.MobileNumber;
            result.ExpireDate = userData.ExpireDate;
            result.InitBalance = userData.InitBalance;

            var apiResult = new ApiResult<PositionModel>(true, ApiResultStatusCode.Success, result);
            return apiResult;

        }

        [HttpGet("[action]")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public async Task<ApiResult<dynamic>> GetHistory()
        {
            var now = DateTime.Now;
            var thisMonth = new DateTime(now.Year, now.Month, 1);
            thisMonth = thisMonth.AddMonths(-100);
            var data = _axPositionLogRepository.GetAll(x => x.ExitTime.HasValue && x.ExitTime >= thisMonth && (x.ProfitPercent > 1 || x.ProfitPercent < -1)).ToList().GroupBy(x => new { x.ExitTime.GetValueOrDefault().Year, x.ExitTime.GetValueOrDefault().Month }).ToList();

            var thisMonthData = data.Select(x => new
            {
                Month = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MMMM"),
                SumOfProfitPercent = x.Sum(t => t.ProfitPercent),
                SumOfProfit = x.Sum(t => t.Profit),
                Trades = x.OrderByDescending(t => t.ExitTime),
                SuccessCount = x.Count(t => t.ProfitPercent > 1),
                FaieldCount = x.Count(t => t.ProfitPercent < -1)
            });

            return Ok(thisMonthData);
        }


    }
    public class PositionModel
    {
        public List<BinancePositionInfoUsdt> List { get; set; } = new();
        public decimal TotalWallet { get; set; }
        public decimal TotalUnPnl { get; set; }
        public decimal TotalMargin { get; set; }
        public string MobileNumber { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal InitBalance { get; set; }
    }
}