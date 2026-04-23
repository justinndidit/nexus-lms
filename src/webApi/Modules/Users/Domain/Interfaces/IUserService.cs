using System;
using webApi.Modules.Auth.Application.Dtos;
using webApi.Modules.Auth.Domain.Interfaces;

namespace webApi.Modules.Users.Domain.Interfaces;

public interface IUserService
{
    public Task<User> GetUserByEmail(string email);
    public Task<(string userId, string email, List<string> roleNames)> CreateUserWithDefaultRole(string email, string passwordHash);
    public Task<bool> UserWithEmailExists(string email);

    public Task<User> UpdateUserPassword(string email, string passwordHash);

    public Task<User> UpdateUserActiveStatus(string email, bool active);
    
}
