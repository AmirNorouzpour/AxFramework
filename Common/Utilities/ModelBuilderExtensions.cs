//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using Pluralize.NET.Core;

//namespace Common.Utilities
//{
//    public static class ModelBuilderExtensions
//    {
//        /// <summary>
//        /// Singularizin table name like Posts to Post or People to Person
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        public static void AddSingularizingTableNameConvention(this ModelBuilder modelBuilder)
//        {
//            Pluralizer pluralizer = new Pluralizer();
//            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
//            {
//                var tableName = entityType.GetTableName();
//                entityType.SetTableName(pluralizer.Pluralize(tableName));
//            }
//        }

//        /// <summary>
//        /// Pluralizing table name like Post to Posts or Person to People
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        public static void AddPluralizingTableNameConvention(this ModelBuilder modelBuilder)
//        {
//            var pluralizer = new Pluralizer();
//            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
//            {
//                var tableName = entityType.GetTableName();
//                entityType.SetTableName(pluralizer.Pluralize(tableName));
//            }
//        }

//        /// <summary>
//        /// Set NEWSEQUENTIALID() sql function for all columns named "Id"
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        public static void AddSequentialGuidForIdConvention(this ModelBuilder modelBuilder)
//        {
//            modelBuilder.AddDefaultValueSqlConvention("Id", typeof(Guid), "NEWSEQUENTIALID()");
//        }

//        /// <summary>
//        /// Set DefaultValueSql for sepecific property name and type
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        /// <param name="propertyName">Name of property wants to set DefaultValueSql for</param>
//        /// <param name="propertyType">Type of property wants to set DefaultValueSql for </param>
//        /// <param name="defaultValueSql">DefaultValueSql like "NEWSEQUENTIALID()"</param>
//        public static void AddDefaultValueSqlConvention(this ModelBuilder modelBuilder, string propertyName, Type propertyType, string defaultValueSql)
//        {
//            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
//            {
//                var property = entityType.GetProperties().SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
//                if (property != null && property.ClrType == propertyType)
//                    property.SetDefaultValueSql(defaultValueSql);
//            }
//        }

//        /// <summary>
//        /// Set DeleteBehavior.Restrict by default for relations
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        public static void AddRestrictDeleteBehaviorConvention(this ModelBuilder modelBuilder)
//        {
//            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
//                .SelectMany(t => t.GetForeignKeys())
//                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
//            foreach (IMutableForeignKey fk in cascadeFKs)
//                fk.DeleteBehavior = DeleteBehavior.Restrict;
//        }

//        /// <summary>
//        /// Dynamicaly load all IEntityTypeConfiguration with Reflection
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        /// <param name="assemblies">Assemblies contains Entities</param>
//        public static void RegisterEntityTypeConfiguration(this ModelBuilder modelBuilder, params Assembly[] assemblies)
//        {
//            var applyGenericMethod = typeof(ModelBuilder).GetMethods().First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

//            var types = assemblies.SelectMany(a => a.GetExportedTypes()).Where(c => c.IsClass && !c.IsAbstract && c.IsPublic);

//            foreach (var type in types)
//            {
//                foreach (var iface in type.GetInterfaces())
//                {
//                    var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
//                    if (iface.IsConstructedGenericType && iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
//                    {
//                        applyConcreteMethod.Invoke(modelBuilder, new[] { Activator.CreateInstance(type) });
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Dynamicaly register all Entities that inherit from specific BaseType
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        /// <param name="assemblies">Assemblies contains Entities</param>
//        public static void RegisterAllEntities<TBaseType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
//        {
//            var types = assemblies.SelectMany(a => a.GetExportedTypes()).Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(TBaseType).IsAssignableFrom(c));
//            var a = types.ToList();
//            foreach (var type in types)
//            {
//                modelBuilder.Entity(type);
//                modelBuilder.Entity(type).Property<byte[]>("RowVersion").IsRowVersion();
//            }
//        }
//    }
//}
