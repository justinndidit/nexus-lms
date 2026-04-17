using System;
using Microsoft.EntityFrameworkCore;
using webApi.Application.Dtos.User.Request;
using webApi.Application.Interfaces;
using webApi.Data;
using webApi.Domain.Dtos.Auth;
using webApi.Domain.Models;

namespace webApi.Application.Repositories;

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

    public async Task<User?> GetUserByEmail(string email) => 
        await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetUserById(Guid userId) => 
        await _dbContext.Users.FindAsync(userId);

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