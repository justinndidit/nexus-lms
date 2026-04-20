using System;
using webApi.Application.Dtos.Auth;
using webApi.Domain.Dtos.Auth;
using webApi.Domain.Models;
using webApi.Modules.Auth.Application.Dtos;
using webApi.Modules.Auth.Domain.Interfaces;
using webApi.Modules.Users.Application.Dtos;

namespace webApi.Application.Interfaces;

public interface IUserService
{
    public Task<User> GetUserByEmail(string email);
    public Task<User> CreateUserWithDefaultRole(CreateUserCommand req);
    public Task<bool> UserWithEmailExists(string email);

    public Task<User> UpdateUserPassword(UpdateUserPasswordCommand cmd);

    public Task<User> UpdateUserActiveStatus(UpdateUserActiveStatusCommand cmd);
    
}
