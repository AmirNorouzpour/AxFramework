using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Utilities;
using Entities.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entitiesAssembly = typeof(IEntity).Assembly;
            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            modelBuilder.AddSequentialGuidForIdConvention();
            modelBuilder.AddPluralizingTableNameConvention();
        }

        public int SaveChanges(AuditType type)
        {
            var result = 0;
            CreateAuditLog(type);
            try
            {
                result = base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                Console.WriteLine(exception);
            }
            return result;
        }

        //public override int SaveChanges(bool acceptAllChangesOnSuccess)
        //{
        //    return base.SaveChanges(acceptAllChangesOnSuccess);
        //}

        public Task<int> SaveChangesAsync(AuditType type, CancellationToken cancellationToken)
        {
            CreateAuditLog(type);
            var result = base.SaveChangesAsync(cancellationToken);
            return result;
        }

        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        //{
        //    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}

        private void CreateAuditLog(AuditType type)
        {
            var entityEntries = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted).ToList();
            foreach (var entry in entityEntries)
            {
                var tableName = entry.Entity.GetType().Name;
                if (tableName != "Audit" && AuditSingleton.AuditTables.Any(x => x == tableName))
                {
                    var creatorUserId = entry.CurrentValues.GetValue<int>("CreatorUserId");
                    var audit = new Audit
                    {
                        CreatorUserId = creatorUserId,
                        InsertDateTime = DateTime.Now,
                        PrimaryKey = type == AuditType.Add ? 0 : entry.CurrentValues.GetValue<int>("Id"),
                        AuditType = type,
                        EntityInsertDateTime = entry.CurrentValues.GetValue<DateTime>("InsertDateTime"),
                        TableName = tableName,
                        UserId = creatorUserId,
                        Value = GetValueAsString(entry, type)
                    };
                    if (!string.IsNullOrWhiteSpace(audit.Value))
                    {
                        AuditSingleton.Audits.Add(audit);
                    }
                }
            }
        }

        private string GetValueAsString(EntityEntry entry, AuditType type)
        {
            var logString = new StringBuilder();
            foreach (var property in entry.Properties.Where(x => x.Metadata.Name != "RowVersion"))
            {
                var value = property.CurrentValue;
                var value2 = property.OriginalValue;
                if (value != null && value2 != null)
                {
                    if (type == AuditType.Update && value.Equals(value2))
                        continue;

                    var columnName = property.Metadata.Name;
                    if (type == AuditType.Update)
                        logString.Append(columnName + " : " + value + " ===> " + value2 + "\r\n");
                    else
                    {
                        if (type == AuditType.Add && columnName == "Id")
                            value = 0;
                        logString.Append(columnName + " : " + value + "\r\n");
                    }
                }
            }

            return logString.ToString();
        }
    }

    public sealed class AuditSingleton
    {
        private static readonly Lazy<List<Audit>> Lazy = new Lazy<List<Audit>>(() => new List<Audit>());
        public static List<Audit> Audits => Lazy.Value;

        private static readonly Lazy<List<string>> LazyAuditTables = new Lazy<List<string>>(() => new List<string>());
        public static List<string> AuditTables => LazyAuditTables.Value;

    }

    public class ConnSingleton
    {
        private static ConnSingleton _instance;

        private ConnSingleton() { }

        public static ConnSingleton Instance
        {
            get { return _instance ??= new ConnSingleton(); }
        }

        public string Value { get; set; }
        public string Name { get; set; }
    }
}
