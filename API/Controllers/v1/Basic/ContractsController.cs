using System.Linq;
using API.Models;
using AutoMapper.QueryableExtensions;
using Common;
using Data.Repositories;
using Entities.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.v1.Basic
{
    [ApiVersion("1")]
    public class ContractsController : BaseController
    {
        private readonly IBaseRepository<Contract> _repository;

        public ContractsController(IBaseRepository<Contract> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AxAuthorize(StateType = StateType.Authorized, Order = 0, AxOp = AxOp.UserList, ShowInMenu = true)]
        public ApiResult<IQueryable<ContractsDto>> Get([FromQuery] DataRequest request)
        {
            var data = _repository.GetAll().Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ProjectTo<ContractsDto>();
            Response.Headers.Add("X-Pagination", _repository.Count().ToString());
            return Ok(data);
        }


    }

}