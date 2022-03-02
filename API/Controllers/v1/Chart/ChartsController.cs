using System;
using System.Collections.Generic;
using Common;
using Data.Repositories;
using Entities.Framework.AxCharts;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using API.Models;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Entities.Framework;
using Entities.Framework.AxCharts.Common;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;
using WebFramework.Filters;
using WebFramework.UserData;

namespace API.Controllers.v1.Chart
{
    [ApiVersion("1")]
    public class ChartsController : BaseController
    {
        private readonly IBaseRepository<AxChart> _repository;
        private readonly IBaseRepository<PieChart> _pieRepository;
        private readonly IBaseRepository<BarChart> _barChartRepository;
        private readonly IBaseRepository<NumericWidget> _numericWidgetRepository;
        private readonly IBaseRepository<LoginLog> _loginlogRepository;
        private readonly IBaseRepository<LineChart> _lineRepository;
        private readonly IBaseRepository<HardwareDataHistory> _hardRepository;

        public ChartsController(IBaseRepository<AxChart> repository, IBaseRepository<PieChart> pieRepository, IBaseRepository<BarChart> barChartRepository,
            IBaseRepository<NumericWidget> numericWidgetRepository, IBaseRepository<LoginLog> loginlogRepository,
            IBaseRepository<LineChart> lineRepository, IBaseRepository<HardwareDataHistory> hardRepository)
        {
            _repository = repository;
            _pieRepository = pieRepository;
            _barChartRepository = barChartRepository;
            _numericWidgetRepository = numericWidgetRepository;
            _loginlogRepository = loginlogRepository;
            _lineRepository = lineRepository;
            _hardRepository = hardRepository;
        }

        [HttpGet("[action]/{chartId}/{filter?}")]
        [AxAuthorize(StateType = StateType.Ignore)]
        public ApiResult<dynamic> GetChart(int chartId, string filter = null, DateTime? date1 = null, DateTime? date2 = null)
        {
            if (date1 == null)
                date1 = DateTime.Now;
            if (date2 == null)
                date2 = DateTime.Now.AddDays(1);

            var chart = _repository.GetAll(x => x.Id == chartId).Include(x => x.Report).ThenInclude(x => x.Filters).FirstOrDefault();

            if (chart?.ChartType == AxChartType.Pie)
            {
                var pieChart = _pieRepository.GetAll(x => x.AxChartId == chartId).ProjectTo<PieChartDto>().FirstOrDefault();
                if (pieChart != null)
                {
                    if (string.IsNullOrWhiteSpace(filter))
                    {
                        var data = _loginlogRepository.GetAll(x => x.InsertDateTime.Date >= date1.Value.Date && x.InsertDateTime <= date2.Value.Date).GroupBy(x => x.Browser)
                            .Select(x => new { x.Key, Count = x.Count() }).ToList();
                        pieChart.Series.Data = data.Select(x => x.Count);
                        pieChart.Labels = data.Select(x => new LegendDto { Name = x.Key, Tag = x.Key }).ToList();
                    }
                    else
                    {
                        var data = _loginlogRepository.GetAll(x => x.InsertDateTime.Date >= date1.Value.Date && x.InsertDateTime <= date2.Value.Date && x.Browser == filter).GroupBy(x => x.BrowserVersion).Select(x => new { x.Key, Count = x.Count() }).ToList();
                        pieChart.Series.Data = data.Select(x => x.Count);
                        pieChart.Labels = data.Select(x => new LegendDto { Name = x.Key }).ToList();
                        pieChart.Title = $"{chart.Title} '{filter}'";
                    }
                }
                return Ok(pieChart);
            }

            if (chart?.ChartType == AxChartType.NumericWidget)
            {
                var numericWidget = _numericWidgetRepository.GetAll(x => x.AxChartId == chartId).ProjectTo<NumericWidgetDto>().FirstOrDefault();
                if (numericWidget != null)
                {
                    var data = chart.Report.Execute();
                    numericWidget.Data = (int)data;
                    numericWidget.LastUpdated = DateTime.Now.ToPerDateTimeString("yyyy/MM/dd HH:mm:ss");
                    return Ok(numericWidget);
                }
            }
            if (chart?.ChartType == AxChartType.Line)
            {

                var lineChart = _lineRepository.GetAll(x => x.AxChartId == chartId).ProjectTo<LineChartDto>().FirstOrDefault();
                var data = _hardRepository.GetAll(x => x.InsertDateTime >= DateTime.Now.AddHours(-2))
                    .OrderBy(x => x.InsertDateTime).ToList();
                if (lineChart != null)
                {
                    lineChart.Series = new List<AxSeriesDto>();
                    for (var i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            lineChart.Series.Add(new AxSeriesDto { Name = "RAM", Id = i });
                            lineChart.Series[i].Data = data.Select(c => c.Ram);
                        }
                        if (i == 1)
                        {
                            lineChart.Series.Add(new AxSeriesDto { Name = "CPU", Id = i });
                            lineChart.Series[i].Data = data.Select(c => c.Cpu);
                        }
                    }
                    lineChart.Labels = data.Select(x => x.InsertDateTime.ToPerDateString("HH:mm")).ToList();
                    return Ok(lineChart);
                }
            }
            if (chart?.ChartType == AxChartType.Bar)
            {
                var barChart = _barChartRepository.GetAll(x => x.AxChartId == chartId).ProjectTo<BarChartDto>().FirstOrDefault();
                if (barChart != null)
                {
                    var date = DateTime.Now.AddDays(-15);
                    var data0 = _loginlogRepository.GetAll(x => x.InsertDateTime.Date >= date.Date).ToList()
                        .GroupBy(x => x.InsertDateTime.Date).OrderBy(x => x.Key).Select(x => new
                        { Count = x.Count(), x.Key, UnScuccessCount = x.Count(t => t.ValidSignIn == false) })
                        .ToList();
                    //var data = chart.Report.Execute();
                    var a = data0.Select(x => x.Count).ToList();
                    var b = data0.Select(x => x.UnScuccessCount).ToList();
                    barChart.Series[0] = new AxSeriesDto { Data = a, Name = "تعداد ورود به سیستم" };
                    barChart.Series.Add(new AxSeriesDto { Data = b, Name = "تعداد ورود ناموفق" });
                    barChart.Labels = data0.Select(x => x.Key.ToPerDateString("d MMMM")).ToList();
                    return Ok(barChart);
                }
            }
            if (chart?.ChartType == AxChartType.List)
            {
                var assembly = typeof(UserDto).Assembly;
                var resultDtoType = assembly.GetType(chart.Report.ResultTypeName);
                var scope = AutoFacSingleton.Instance.BeginLifetimeScope();
                var data0 = chart.Report.Execute(scope);
                var data = typeof(ReportExtensions).GetMethod("AxProjectTo")?.MakeGenericMethod(resultDtoType).Invoke(null, new[] { data0 });
                if (data is IQueryable<dynamic> data2)
                    data = data2.ToList();
                var columns = resultDtoType.GetCustomAttributesOfType(new List<string> { "Id" });
                var listChart = new ListChartDto { Id = chartId, Title = chart.Title, NextChartId = chart.NextChartId, NextChartType = chart.NextChartType, Data = data, Columns = columns };
                scope.Dispose();
                return Ok(listChart);
            }
            return Ok(chart);
        }

    }
}