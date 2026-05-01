using System;

namespace webApi.Modules.Rbac.Domain.Models;

public class UserRole
{
    public Guid UserId{get; private set;}
    public Guid RoleId{get; private set;}

    public Role Role{get; private set;} = null!;

    private UserRole() {}

    public UserRole(Guid roldeId, Guid userId)
    {
        UserId = userId;
        RoleId = roldeId;
    }
}
