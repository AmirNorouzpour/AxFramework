using System;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Tracking;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data.Repositories;
using Entities.Framework.Reports;
using Entities.Tracking;
using Microsoft.AspNetCore.Mvc;
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
        [AxAuthorize(StateType = StateType.Authorized, AxOp = AxOp.ProductInstanceHistoryList)]
        public virtual ApiResult<IQueryable<ProductInstanceHistoryDto>> Get([FromQuery] DataRequest request, string code = null, long? op = null, string userIds = null, DateTime? date = null)
        {

            var data0 = _repository.GetAll();
            if (!string.IsNullOrWhiteSpace(code))
                data0 = data0.Where(x => x.ProductInstance.Code == code);

            if (userIds != null)
            {
                var userIdsArray = userIds.Split(',').Select(int.Parse);
                data0 = data0.Where(x => userIdsArray.Contains(x.ProductInstance.Personnel.UserId));
            }

            if (date.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date == date.Value.Date);

            if (op.HasValue)
                data0 = data0.Where(x => x.OpId == op);

            var data = data0.OrderBy(request.Sort, request.SortType).OrderByDescending(x => x.Id).Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<ProductInstanceHistoryDto>();
            Response.Headers.Add("X-Pagination", data0.Count().ToString());
            return Ok(data);
        }

        [HttpGet("ExportToXlsx")]
        [AxAuthorize(StateType = StateType.Ignore, AxOp = AxOp.ProductInstanceHistoryList)]
        public Task<FileContentResult> ExportToXlsx(string code = null, long? op = null, string userIds = null, DateTime? date = null)
        {
            var data0 = _repository.GetAll();
            if (!string.IsNullOrWhiteSpace(code))
                data0 = data0.Where(x => x.ProductInstance.Code == code);

            if (userIds != null)
            {
                var userIdsArray = userIds.Split(',').Select(int.Parse);
                data0 = data0.Where(x => userIdsArray.Contains(x.ProductInstance.Personnel.UserId));
            }

            if (date.HasValue)
                data0 = data0.Where(x => x.InsertDateTime.Date == date.Value.Date);

            if (op.HasValue)
                data0 = data0.Where(x => x.OpId == op);

            var data = data0.OrderByDescending(x => x.Id).ProjectTo<ProductInstanceHistoryDto>();

            var name = DateTime.Now.ToPerDateTimeString();
            var fileName = name + ".xlsx";
            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileBytes = GeneralUtils.ExportToExcel(data.ToList());

            return Task.FromResult(new FileContentResult(fileBytes.ToArray(), mimeType)
            {
                FileDownloadName = fileName
            });

        }




    }
}
