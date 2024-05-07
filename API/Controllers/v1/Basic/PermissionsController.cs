using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
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
    [ApiVersion("1")]
    public class PermissionsController : BaseController
    {
        private readonly IBaseRepository<Permission> _repository;
        private readonly IBaseRepository<Menu> _menuRepository;
        private readonly IBaseRepository<UserGroup> _userGroupRepository;
        private readonly IBaseRepository<UserSetting> _userSettingRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IMemoryCache _memoryCache;

        public PermissionsController(IBaseRepository<Permission> repository, IBaseRepository<Menu> menuRepository,
            IMemoryCache memoryCache, IBaseRepository<UserGroup> userGroupRepository, IBaseRepository<UserSetting> userSettingRepository, IBaseRepository<User> userRepository)
        {
            _repository = repository;
            _menuRepository = menuRepository;
            _memoryCache = memoryCache;
            _userGroupRepository = userGroupRepository;
            _userSettingRepository = userSettingRepository;
            _userRepository = userRepository;
        }

        [HttpGet("[action]/{id}")]
        [AxAuthorize(StateType = StateType.Authorized, ShowInMenu = true, AxOp = AxOp.PermissionTree, Order = 2)]
        public ApiResult<dynamic> GetTree(int id)
        {
            var permissions = _menuRepository.GetAll(x => x.Active)
                .Include(x => x.Children).ThenInclude(x => x.Children).ThenInclude(x => x.Children)
                .ThenInclude(x => x.Children);
            var newData = SerializeData(permissions.Where(x => x.ParentId == null).ToList(), id);
            return Ok(newData);
        }

        private ICollection<dynamic> SerializeData(List<Menu> data, int userId)
        {
            userId = userId == 0 ? UserId : userId;
            var keys = _memoryCache.GetOrCreate("user" + userId, entry => PermissionHelper.GetKeysFromDb(_repository, _userGroupRepository, userId));
            return data?.Where(x => x.Active).Select(x => new
            {
                x.Key,
                x.Title,
                Children = x.Children != null && x.Children.Any() ? SerializeData(x.Children.ToList(), userId) : null,
                IsLeaf = x.Children == null || x.Children.Count == 0,
                @checked = keys.Any(t => t == x.Key) ? true : (bool?)null,
            }).ToArray();
        }


        [HttpPost("[action]/{ugType}/{id}")]
        [AxAuthorize(StateType = StateType.Authorized, ShowInMenu = false, AxOp = AxOp.PermissionTreeSave, Order = 0)]
        public ApiResult<dynamic> SavePermissions(UgType ugType, int id, List<Menu> data)
        {
            if (ugType == UgType.User)
            {
                SetPermissions(id, data, true);
            }
            else
            {
                var users = _userGroupRepository.GetAll(x => x.GroupId == id);
                foreach (var item in users)
                {
                    SetPermissions(item.UserId, data, false);
                }
            }

            return Ok();
        }

        private void SetPermissions(int id, List<Menu> data, bool deleteOld)
        {
            var user = _userRepository.GetById(id);
            var userSetting = _userSettingRepository.GetFirst(x => x.UserId == id);
            if (user != null && data.Count > 0)
            {
                if (deleteOld)
                {
                    var userOldPermissions = _repository.GetAll(x => x.UserId == id);
                    _repository.DeleteRange(userOldPermissions);
                }

                foreach (var m in data)
                {
                    var children = Traverse(m).ToList();
                    foreach (var p in children)
                    {
                        if (!p.Checked /*|| !p.IsLeaf*/)
                            continue;
                        var menu = _menuRepository.GetFirst(x => x.Key == p.Key);
                        var newPermission = new Permission
                        {
                            UserId = id,
                            Access = p.Checked,
                            InsertDateTime = DateTime.Now,
                            MenuId = menu.Id,
                            CreatorUserId = UserId
                        };
                        _repository.Add(newPermission);
                    }
                }

                _memoryCache.Remove("user" + id);
                var system = _menuRepository.GetFirst(x => x.Name == "Basic");
                if (userSetting != null)
                {
                    userSetting.DefaultSystemId = system.Id;
                    _userSettingRepository.Update(userSetting);
                }
                else
                {
                    _userSettingRepository.Add(new UserSetting
                    {
                        UserId = id,
                        CreatorUserId = 1,
                        DefaultSystemId = system.Id,
                        InsertDateTime = DateTime.Now,
                        Theme = "light",
                    });
                }
            }
        }

        public static IEnumerable<Menu> Traverse(Menu root)
        {
            var stack = new Stack<Menu>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                if (current.Children != null)
                    foreach (var child in current.Children)
                        stack.Push(child);
            }
        }

    }
}