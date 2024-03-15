//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Autofac;
//using AutoMapper.QueryableExtensions;
//using Common.Utilities;
//using Data.Repositories;
//using Entities.Framework;
//using Entities.Framework.Reports;
//using ExpressionBuilder.Generics;

//namespace WebFramework.UserData
//{
//    public static class ReportExtensions
//    {
//        public static object Execute0<T>(this Report report, ILifetimeScope scope = null) where T : BaseEntity
//        {
//            object data;
//            if (scope == null)
//                scope = AutoFacSingleton.Instance.BeginLifetimeScope();

//            var repository = scope.Resolve<IBaseRepository<T>>();
//            Filter<T> predicate = null;
//            if (report.Filters != null && report.Filters.Any())
//            {
//                var request = new DataRequest { Filters = new List<AxFilter>() };
//                request.Filters.AddRange(report.Filters);
//                predicate = request.GetFilter<T>();
//            }

//            switch (report.ExecuteType)
//            {
//                case ReportType.All:
//                    {
//                        data = !string.IsNullOrWhiteSpace(report.Sort) ? repository.GetAll(predicate).OrderBy(report.Sort, report.SortType).Take(report.TakeSize > 0 ? report.TakeSize : int.MaxValue) : repository.GetAll(predicate).Take(report.TakeSize > 0 ? report.TakeSize : int.MaxValue);
//                        if (!string.IsNullOrWhiteSpace(report.GroupBy) && data is IQueryable<T> queryable)
//                        {
//                            var lambda = GroupByUtility.GroupByExpression<T>(new[] { report.GroupBy });
//                            var func = lambda.Compile();
//                            data = queryable.AsEnumerable().GroupBy(func);
//                        }
//                    }
//                    break;
//                case ReportType.First:
//                    data = !string.IsNullOrWhiteSpace(report.Sort) ? repository.GetAll(predicate).OrderBy(report.Sort, report.SortType).Take(1) : repository.GetAll(predicate).Take(1);
//                    break;
//                case ReportType.Count:
//                    data = repository.Count(predicate);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException($"report '{report.Title}' executeType is not valid");
//            }
//            return data;
//        }

//        public static object Execute(this Report report, ILifetimeScope scope = null)
//        {
//            var assembly = typeof(BaseEntity).Assembly;
//            var type = assembly.GetType(report.TypeName);
//            var data = typeof(ReportExtensions).GetMethod("Execute0")?.MakeGenericMethod(type).Invoke(null, new object[] { report, scope });
//            return data;
//        }

//        public static object AxProjectTo<T>(this object data)
//        {
//            if (data is IQueryable queryable)
//                return queryable.ProjectTo<T>();

//            return data;
//        }


//    }
//}
