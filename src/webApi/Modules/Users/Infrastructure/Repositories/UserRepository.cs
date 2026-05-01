using System;
using Microsoft.EntityFrameworkCore;
using webApi.Data;
using webApi.Modules.Rbac.Domain.Models;
using webApi.Modules.Users.Domain.Interfaces;
using webApi.Modules.Users.Domain.Models;

namespace webApi.Modules.Users.Infrastructure.Repositories;

public class UserRepository(LMSApiApplicationContext dbContext) : IUserRepository
{
    private readonly LMSApiApplicationContext _dbContext = dbContext;

    public async Task<User> CreateUser(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task AssignUserRoles(Guid userId, List<Guid> roleIds)
    {
        var userRoles = roleIds.Select(roleId => new UserRole(roleId, userId));
        await _dbContext.UserRoles.AddRangeAsync(userRoles);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> GetUserByEmail(string email) =>
        await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new KeyNotFoundException("user not found");

    public async Task<User> GetUserById(Guid userId) =>
        await _dbContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("user net found");

    public async Task<User> UpdateUser(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async void DeleteUser(User user)
    {
     _dbContext.Users.Remove(user);
     await _dbContext.SaveChangesAsync();
    }
    // public async Task CreateUserTx(User user)
    // {
    //     await _dbContext.Users.AddAsync(user);
    // }

    // public async Task AssignUserRolesTx(Guid userId, List<Guid> roleIds)
    // {
    //     var userRoles = roleIds.Select(roleId => new UserRole(roleId, userId));
    //     await _dbContext.UserRoles.AddRangeAsync(userRoles);
    // }
}