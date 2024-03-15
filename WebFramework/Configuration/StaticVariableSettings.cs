//using System.Linq;
//using Data;
//using Data.Repositories;
//using Entities.Framework;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;

//namespace WebFramework.Configuration
//{
//    public static class StaticVariableSettings
//    {
//        public static void UseSetStaticVariables(this IApplicationBuilder app)
//        {
//            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
//            var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
//            var auditTablesRepository = context.GetService<IBaseRepository<AuditTable>>();
//            AuditSingleton.AuditTables.AddRange(auditTablesRepository.GetAll(x => x.Active).Select(x => x.TableName).ToList());
//        }
//    }
//}