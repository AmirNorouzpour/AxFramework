using System.Linq;
using System.Net;
using System.Security.Claims;
using Common;
using Common.Exception;
using Common.Utilities;
using Data.Repositories;
using Entities.Framework;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using WebFramework.UserData;

namespace WebFramework.Filters
{
    public class AxAuthorizeAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public AxOp AxOp { get; set; }
        public AxOp ParentAxOp => this.AxOp.GetAxParent();
        public bool ShowInMenu { get; set; }

        public StateType StateType { get; set; }

        public AxAuthorizeAttribute(AxOp axOp, bool showInMenu = false)
        {
            AxOp = axOp;
            ShowInMenu = showInMenu;
        }

        public AxAuthorizeAttribute()
        {

        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var context = filterContext.HttpContext;
            var memoryCache = filterContext.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            var userId = context.User.Identity.GetUserId<int>();
            if (StateType == StateType.OnlyToken && userId <= 0) 
                throw new AppException(ApiResultStatusCode.UnAuthenticated, "شما برای دسترسی به منابع مورد نظر احراز هویت نشده اید", HttpStatusCode.Unauthorized);

            if (StateType == StateType.Authorized)
            {
                //var keys = memoryCache.GetOrCreate("user" + userId, entry =>
                //{
                //    var permissionRepository = filterContext.HttpContext.RequestServices.GetRequiredService<IBaseRepository<Permission>>();
                //    var userGroupRepository = filterContext.HttpContext.RequestServices.GetRequiredService<IBaseRepository<UserGroup>>();
                //    var data = PermissionHelper.GetKeysFromDb(permissionRepository, userGroupRepository, userId);
                //    return data;
                //}).ToList();
                //var axKey = AxOp.GetAxKey();
                //var haveAccess = keys.Any(x => x.ToLower() == axKey);

                //if (!haveAccess)
                //{
                //    if (StateType == StateType.CheckParent)
                //    {
                //        var haveAccessToParent = keys.Any(key => key == ParentAxOp.GetAxKey());
                //        if (!haveAccessToParent)
                //        {
                //            throw new AppException(ApiResultStatusCode.UnAuthorized, "شما دسترسی برای عملیات درخواست شده را ندارید", HttpStatusCode.Unauthorized);
                //        }
                //    }
                //    else
                //        throw new AppException(ApiResultStatusCode.UnAuthorized, "شما دسترسی برای عملیات درخواست شده را ندارید", HttpStatusCode.Unauthorized);
                //}
            }
        }
    }
}
