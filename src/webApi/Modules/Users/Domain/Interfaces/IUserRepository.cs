using webApi.Modules.Users.Domain.Models;

namespace webApi.Modules.Users.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserById(Guid userId);
    Task<User> GetUserByEmail(string email);
    Task<User> CreateUser(User user);
    Task<User> UpdateUser(User user);
    Task AssignUserRoles(Guid userId, List<Guid> roleIds);
    // Task AssignUserRolesTx(Guid userId, List<Guid> roleIds);
    // Task CreateUserTx(User user);

}