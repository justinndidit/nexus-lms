
namespace webApi.Modules.Users.Application.Services;

public class UserService : IUserService
{
    private readonly LMSApiApplicationContext _dbContext;
    private readonly ILogger<UserService> _logger;
    private readonly string _defaultUserRoleName = "student";
    
    private readonly IUserRepository _userRepo;
    private readonly IRbacService _rbacService;
    public UserService(
                        LMSApiApplicationContext dbContext,
                        ILogger<UserService> logger,
                        IUserRepository userRepo,
                        IRbacService rbacService
                    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _userRepo = userRepo;
        _rbacService = rbacService;
    }

    public async Task<(string userId, string email, List<string> roleNames)> CreateUserWithDefaultRole(string email, string passwordHash)
    {
        if (await _userRepo.GetUserByEmail(email) is not null)
            throw new InvalidOperationException($"user with Email {email} already exists");  

        User user = await _userRepo.CreateUser(
            new User(email, passwordHash)
        );
        _logger.LogInformation("User created successfully!");

        await _rbacService.AssignUserToRole(user.Id, _defaultUserRoleName);
        _logger.LogInformation("default role assigned to user");

        return (user.Id.ToString(), user.Email, user.UserRoles.Select(ur => ur.Role.RoleName).ToList());
    }
    
    public async Task<User> GetUserByEmail(string email)
    {
        return 
            await _userRepo.GetUserByEmail(email)
            ?? throw new KeyNotFoundException("User not found");
    }

    public async Task<User> UpdateUserPassword(string email, string passwordHash)
    {
        try
        {
            User user = await GetUserByEmail(email);
            user.PasswordHash = passwordHash;
            return await _userRepo.UpdateUser(user);   
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"error occured whilst processing request");
            throw;
        }
    }

    public async Task<User> UpdateUserActiveStatus(string email, bool activate)
    {  
       var user = await GetUserByEmail(email);
        if (user.IsActive == activate) return user;

        if (activate) user.Activate();
        else user.Deactivate();
        return await _userRepo.UpdateUser(user);
    }

    public async Task<bool> UserWithEmailExists(string email)
    {
        return await _userRepo.GetUserByEmail(email) is not null;
    }
}

