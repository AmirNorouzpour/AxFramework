using System.Collections.Generic;
using System.Linq;
using API.Models;
using AutoMapper.QueryableExtensions;
using Common;
using Data.Repositories;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebFramework.Api;
using WebFramework.Filters;
using WebFramework.UserData;

namespace API.Controllers.v1.Basic
{
    /// <summary>
    /// Fetch Menus or Systems for user 
    /// </summary>
    [ApiVersion("1")]
    public class MenusController : BaseController
    {
        private readonly IBaseRepository<Menu> _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly IBaseRepository<Permission> _permissionRepository;
        private readonly IBaseRepository<UserGroup> _userGroupRepository;
        private readonly IBaseRepository<UserChart> _userChartsRepository;

        public MenusController(IBaseRepository<Menu> repository, IMemoryCache memoryCache, IBaseRepository<Permission> permissionRepository, IBaseRepository<UserGroup> userGroupRepository, IBaseRepository<UserChart> userChartsRepository)
        {
            _repository = repository;
            _memoryCache = memoryCache;
            _permissionRepository = permissionRepository;
            _userGroupRepository = userGroupRepository;
            _userChartsRepository = userChartsRepository;
        }

        /// <summary>
        /// Get Systems And Menus for Signed User
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{systemId}")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public virtual ApiResult<List<MenuDto>> GetSystemMenus(int systemId)
        {
            var menus = _repository.GetAll(x => x.ParentId == systemId && x.Active).OrderBy(x => x.OrderId).Include(x => x.Children).Select(x => new
            {
                x.Key,
                x.Title,
                x.Icon,
                Children = x.Children.Where(c => c.ShowInMenu && x.Active).OrderBy(x => x.OrderId).Select(c => new
                {
                    c.Key,
                    c.Title,
                    c.Icon,
                    c.OrderId
                })
            }).ProjectTo<MenuDto>();

            var keys = _memoryCache.GetOrCreate("user" + UserId, entry => PermissionHelper.GetKeysFromDb(_permissionRepository, _userGroupRepository, UserId));
            if (keys == null)
                return NotFound();

            var lst = menus.ToList();
            var output = new List<MenuDto>();
            foreach (var menu in lst)
            {
                var childList = new List<MenuDto>();
                foreach (var child in menu.Children)
                {
                    if (keys.Any(x => x.Equals(child.Key)))
                        childList.Add(child);
                }
                output.Add(new MenuDto { Children = childList, Icon = menu.Icon, Key = menu.Key, Title = menu.Title });
            }
            return Ok(output);
        }

        [HttpGet("[action]/{systemId}")]
        [AxAuthorize(StateType = StateType.OnlyToken)]
        public virtual ApiResult<dynamic> GetDashboardCharts(int systemId)
        {
            var charts = _userChartsRepository.GetAll(x => x.Active && x.AxChart.Active && x.AxChart.SystemId == systemId).Include(x => x.AxChart).OrderBy(x => x.OrderIndex).Select(x => new
            {
                x.AxChart.Title,
                x.AxChart.ReportId,
                x.AxChart.ChartType,
                x.Width,
                x.Height,
                x.OrderIndex,
                x.AxChart.Id,
                x.AxChart.IsLive
            });
            return Ok(charts);
        }
    }
}