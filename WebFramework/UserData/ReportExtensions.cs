using System.Linq;
using AutoMapper.QueryableExtensions;

namespace WebFramework.UserData
{
    public static class ReportExtensions
    {
        public static object AxProjectTo<T>(this object data)
        {
            if (data is IQueryable queryable)
                return queryable.ProjectTo<T>();

            return data;
        }


    }
}
