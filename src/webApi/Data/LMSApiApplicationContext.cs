using System;
using Microsoft.EntityFrameworkCore;
using webApi.Domain.Models;
using webApi.Modules.Auth.Domain.Models;

namespace webApi.Data;

public class LMSApiApplicationContext(DbContextOptions<LMSApiApplicationContext> options)
                                             : DbContext(options) 
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var idProperty = entity.FindProperty("Id");
            if(idProperty !=null && idProperty.ClrType == typeof(Guid))
            {
                idProperty.SetDefaultValueSql("NEWSEQUENTIALID()");
            }

            modelBuilder.Entity<UserRole>().HasKey(ur=> new {ur.UserId, ur.RoleId});
            modelBuilder.Entity<RolePermission>().HasKey(rp=> new {rp.RoleId, rp.PermissionId});
        }
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<VerificationToken> VerificationTokens => Set<VerificationToken>();
    // public DbSet<DisabledUser> DisabledUsers => Set<DisabledUser>();
}
