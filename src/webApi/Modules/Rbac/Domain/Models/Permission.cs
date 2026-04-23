using System.ComponentModel.DataAnnotations;

namespace webApi.Modules.Rbac.Domain.Models;

public class Permission
{
    [Key]
    public Guid Id { get; private set; }
    public required string Name { get; set; }
    public required string  Description{get; set;}

    private Permission() {}

    public Permission(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
