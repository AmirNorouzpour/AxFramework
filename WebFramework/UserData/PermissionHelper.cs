//using System.Collections.Generic;
//using Entities.Framework;

//namespace WebFramework.UserData
//{
//    public class PermissionHelper
//    {
//        public static HashSet<string> GetKeysFromDb(IBaseRepository<Permission> permissionRepository, IBaseRepository<UserGroup> userGroupRepository, int userId)
//        {
//            var hashSet = new HashSet<string>();
//            var userPermissions = permissionRepository.GetAll(x => x.Access && x.UserId == userId).Include(x => x.Menu);
//            foreach (var item in userPermissions)
//            {
//                if (!string.IsNullOrWhiteSpace(item.Menu.Key))
//                    hashSet.Add(item.Menu.Key);
//            }

//            var userGroups = userGroupRepository.GetAll(x => x.UserId == userId);
//            foreach (var item in userGroups)
//            {
//                var groupPermissions = permissionRepository.GetAll(x => x.GroupId == item.GroupId && x.Access)
//                    .Include(x => x.Menu);
//                foreach (var groupPermission in groupPermissions)
//                {
//                    if (!string.IsNullOrWhiteSpace(groupPermission.Menu.Key))
//                        hashSet.Add(groupPermission.Menu.Key);
//                }
//            }

//            var userDenied = permissionRepository.GetAll(x => x.UserId == userId && !x.Access)
//                .Select(x => x.Menu.Key);
//            foreach (var item in userDenied)
//            {
//                hashSet.Remove(item);
//            }

//            //var NotShowInTreeKeys = _permissionRepository.GetAll(x => !x.ShowInTree && keys.Contains(x.ParentKey) && !x.Key.Contains("GetList")).ToList().Select(x=>x.Key);
//            //foreach (var item in NotShowInTreeKeys)
//            //{
//            //    keys.Add(item);
//            //}
//            return hashSet;

//        }
//    }
//}
