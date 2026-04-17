using System;
using webApi.Application.Dtos.Auth;
using webApi.Domain.Models;

namespace webApi.Application.Common.Mappers;

public static class UserMapper
{
    public static List<string> ToUserRoles(User user)
    {
        List<string> roleNames = [];
        foreach(var userRole in user.UserRoles)
        {
            roleNames.Add(userRole.Role.RoleName!);
        }
        return roleNames;
    }

}
