﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Models.Tracking;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data.Repositories;
using Entities.Framework.Reports;
using Entities.Tracking;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Tracking
{
    [ApiVersion("1")]
    public class ProductInstanceHistoriesController : BaseController
    {
        private readonly IBaseRepository<ProductInstanceHistory> _repository;

        public ProductInstanceHistoriesController(IBaseRepository<ProductInstanceHistory> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.Ignore, AxOp = AxOp.ProductInstanceHistoryList)]
        public virtual ApiResult<IQueryable<ProductInstanceHistoryDto>> Get([FromQuery] DataRequest request, string code = null, long? op = null, long? line = null, long? machine = null, string userIds = null, DateTime? date1 = null, DateTime? date2 = null)
        {

            var data0 = _repository.GetAll();
            if (!string.IsNullOrWhiteSpace(code))
                data0 = data0.Where(x => x.ProductInstance.Code == code);

            if (userIds != null)
            {
                var userIdsArray = userIds.Split(',').Select(int.Parse).ToList();
                data0 = data0.Where(x => userIdsArray.Contains(x.UserId));
            }

            if (date1.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date >= date1.Value.Date);

            if (date2.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date <= date2.Value.Date);


            if (op.HasValue)
                data0 = data0.Where(x => x.Machine.OperationStationId == op);
            if (line.HasValue)
                data0 = data0.Where(x => x.Machine.OperationStation.ProductLineId == line);
            if (machine.HasValue)
                data0 = data0.Where(x => x.MachineId == machine);
            var data = data0.OrderBy(request.Sort, request.SortType).OrderByDescending(x => x.Id).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<ProductInstanceHistoryDto>();
            Response.Headers.Add("X-Pagination", data0.Count().ToString());
            return Ok(data);
        }

        [HttpGet("ExportToXlsx")]
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.ProductInstanceHistoryList)]
        public Task<FileContentResult> ExportToXlsx(string code = null, long? op = null, string userIds = null, DateTime? date1 = null, DateTime? date2 = null)
        {
            var data0 = _repository.GetAll();
            if (!string.IsNullOrWhiteSpace(code))
                data0 = data0.Where(x => x.ProductInstance.Code == code);

            if (userIds != null)
            {
                var userIdsArray = userIds.Split(',').Select(int.Parse);
                data0 = data0.Where(x => userIdsArray.Contains(x.UserId));
            }

            if (date1.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date >= date1.Value.Date);

            if (date2.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date <= date2.Value.Date);

            if (op.HasValue)
                data0 = data0.Where(x => x.Machine.OperationStationId == op);

            var data = data0.OrderByDescending(x => x.Id).ProjectTo<ProductInstanceHistoryDto>();

            var name = DateTime.Now.ToPerDateTimeString();
            var fileName = name + ".xlsx";
            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileBytes = GeneralUtils.ExportToExcel(data.ToList(), "Id,Code,ShiftName,MachineName,OpName,PersonnelName,Username,Date,Time");

            return Task.FromResult(new FileContentResult(fileBytes.ToArray(), mimeType)
            {
                FileDownloadName = fileName
            });

        }

        [HttpGet("Chart")]
        [AxAuthorize(StateType = StateType.Ignore, AxOp = AxOp.ProductInstanceHistoryAnalysis,ShowInMenu = true,Order = 5)]
        public BarChartDto Chart(long? machine = null, int? userId = null, DateTime? date1 = null, DateTime? date2 = null, long? line = null)
        {
            var barChart = new BarChartDto { Series = new List<AxSeriesDto>() };
            var data0 = _repository.GetAll();
            if (date1.HasValue)
                data0 = data0.Where(x => x.EnterTime >= date1);
            if (date2.HasValue)
                data0 = data0.Where(x => x.EnterTime <= date2);

            if (line.HasValue)
            {
                var data = data0.Where(x => x.Machine.OperationStation.ProductLineId == line)
                     .ToList().GroupBy(x => x.EnterTime.Date)
                     .Select(x => new { Count = x.Count(), x.Key, Data = x })
                     .ToList();
                var count = data.Select(x => x.Count)
                    .ToList();
                barChart.Series.Add(new AxSeriesDto { Data = count, Name = "گزارش تولید" });
                barChart.Labels = data.Select(x => x.Key.ToPerDateTimeString("yyyy/MM/dd")).ToList();
                return barChart;
            }

            if (machine.HasValue)
            {
                var data = data0.Where(x => x.MachineId == machine)
                    .ToList().GroupBy(x => x.EnterTime.Date)
                    .Select(x => new { Count = x.Count(), x.Key, Data = x })
                    .ToList();
                var count = data.Select(x => x.Count)
                    .ToList();
                barChart.Series.Add(new AxSeriesDto { Data = count, Name = "گزارش تولید" });
                barChart.Labels = data.Select(x => x.Key.ToPerDateTimeString("yyyy/MM/dd")).ToList();
                return barChart;
            }

            if (userId.HasValue)
            {
                var data = data0.Where(x => x.UserId == userId).ProjectTo<ProductInstanceHistoryDto>().OrderBy(x=> x.Id)
                      .ToList().GroupBy(x => x.ShiftDate)
                      .Select(x => new { Count = x.Count(), x.Key, Data = x })
                      .ToList();
                var count = data.Select(x => x.Count)
                    .ToList();
                barChart.Series.Add(new AxSeriesDto { Data = count, Name = "گزارش تولید" });
                barChart.Labels = data.Select(x => x.Key).ToList();
                return barChart;
            }

            return new BarChartDto();
        }
    }
}
