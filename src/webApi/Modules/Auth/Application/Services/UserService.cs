using System;
using webApi.Application.Dtos.Auth;
using webApi.Application.Interfaces;
using webApi.Application.Repositories;
using webApi.Data;
using webApi.Domain.Dtos.Auth;
using webApi.Domain.Models;
using webApi.Modules.Auth.Domain.Interfaces;
using webApi.Modules.Users.Application.Dtos;

namespace webApi.Application.Services;

public class UserService : IUserService
{
    private readonly LMSApiApplicationContext _dbContext;
    private readonly ILogger<UserService> _logger;
    private readonly string _defaultUserRoleName = "student";
    
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;

    public UserService(
                        LMSApiApplicationContext dbContext,
                        ILogger<UserService> logger,
                        IUserRepository userRepo,
                        IRoleRepository roleRepo
                    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _userRepo = userRepo;
        _roleRepo = roleRepo;
    }

    public async Task<User> CreateUserWithDefaultRole(CreateUserCommand cmd)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();
        _logger.LogInformation("db transaction started successfully");
        try
        {
            if (_userRepo.GetUserByEmail(cmd.Email) is not null)
                throw new BadHttpRequestException($"user with Email {cmd.Email} already exists");  

            User user = await _userRepo.CreateUser(
                new User(cmd.Email, cmd.PasswordHash)
            );
            _logger.LogInformation("User created successfully!");

            Role role = await _roleRepo.GetByNameAsync(_defaultUserRoleName)
                                ?? throw new Exception($"default user role {_defaultUserRoleName} not in database!");
             _logger.LogInformation($"role {role.Id} fetched successfully!");

            await _userRepo.AssignUserRoles(
                user.Id,
                new List<Guid>{}
            );
             _logger.LogInformation($"default role assigned to user {user.Id} successfully!");
            
            await transaction.CommitAsync();
            
            //send Email async - worker
            //generate verification token
            //populate email data
            //send email
            //emit event
            
            return user;
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing: {e.Message}");
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<User> GetUserByEmail(string email)
    {
        return 
            await _userRepo.GetUserByEmail(email)
            ?? throw new DllNotFoundException($"user with Email {email} not found!");
    }

    public async Task<User> UpdateUserPassword(UpdateUserPasswordCommand cmd)
    {
        try
        {
            User user = await GetUserByEmail(cmd.Email);
            user.PasswordHash = cmd.PasswordHash;
            return await _userRepo.UpdateUser(user);   
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }

    public async Task<bool> UserWithEmailExists(string email)
    {
        return await _userRepo.GetUserByEmail(email) is not null;
    }
}

