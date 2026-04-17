using System;
using webApi.Domain.Models;

namespace webApi.Modules.Auth.Domain.Models;

public class DisabledUser
{
    public Guid UserId{get; set;}
    public Guid DisabledById{get;set;}

    public string DisabledReason {get; set;} = string.Empty;
    public DateTime DisabledAt {get; set;}

    private DisabledUser(){}
}
