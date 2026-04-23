using System.ComponentModel.DataAnnotations;

namespace webApi.Modules.Rbac.Domain.Models;

public class Role
{
    [Key]
    public Guid Id{get; private set;}
    public string RoleName{get; set;} = string.Empty;
    public List<RolePermission> RolePermissions{get; private set;} = [];
    private Role(){}

    public Role(string name)
    {
        RoleName = name;
    }
}
