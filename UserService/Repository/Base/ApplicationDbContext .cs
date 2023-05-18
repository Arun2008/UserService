using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using UserService.Models;
using UserService.Models.DBModel;

namespace UserService.Repository.Base
{
    public class DBContext : IdentityDbContext<ApplicationUser, ApplicationUserRole, string>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Ignore AspNetIdentity Table because its created from Authenticate service
            //builder.Ignore<ApplicationUser>();
            //builder.Ignore<ApplicationUserRole>();
            //builder.Ignore<IdentityUserToken<string>>();
            //builder.Ignore<IdentityUserRole<string>>();
            //builder.Ignore<IdentityUserLogin<string>>();
            //builder.Ignore<IdentityUserClaim<string>>();
            //builder.Ignore<IdentityRoleClaim<string>>();

            #endregion
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations());
            builder.Entity<ApplicationUserRole>().ToTable("AspNetRoles", t => t.ExcludeFromMigrations());
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", t => t.ExcludeFromMigrations());
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", t => t.ExcludeFromMigrations());
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", t => t.ExcludeFromMigrations());
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", t => t.ExcludeFromMigrations());
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", t => t.ExcludeFromMigrations());
            builder.Entity<AuditLog>().ToTable("AuditLogs", t => t.ExcludeFromMigrations());

            
        }
        

        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Permission> Permissions => Set<Permission>();
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
