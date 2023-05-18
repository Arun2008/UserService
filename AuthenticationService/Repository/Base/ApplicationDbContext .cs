using AuthenticationService.Models.Common;
using AuthenticationService.Models.DBModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Emit;
using UserService.Models;

namespace AuthenticationService.Repository.Base
{
    public class DBContext : IdentityDbContext<ApplicationUser, ApplicationUserRole, string>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Seeding a  'Administrator' role to AspNetRoles table
            builder.Entity<ApplicationUserRole>().HasData(new ApplicationUserRole { Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "ADMIN".ToUpper(), Status = Status.Active });
        }
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public virtual async Task<int> SaveChangesAsync()
        {
            OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync();
            return result;
        }
        private void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Modified)
                    continue;
                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name
                };
                var changeEntity = (BaseModel)entry.Entity;
                auditEntry.UserId = changeEntity.LastModifierUserId;
                auditEntries.Add(auditEntry);
                foreach (PropertyEntry property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey() && property.CurrentValue != null)
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    if (property.IsModified && property.CurrentValue != null && property.OriginalValue != null)
                    {
                        auditEntry.AuditType = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
        }
    }
}
