using System;

namespace webApi.Modules.Auth.Domain.Models;

public class VerificationToken
{
    public Guid Id {set; get;}
    public string Email{private set; get;} =string.Empty;
    public string Token{set; get;} = string.Empty;
    public DateTime ExpiryDate{set; get;}
    public bool IsValid{get;set;} = true;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get;set;}

    public VerificationToken(string email, string token)
    {
        Email = email;
        Token = token;
        CreatedAt = DateTime.UtcNow;
        ExpiryDate = DateTime.UtcNow.AddMinutes(30);
    }
    private VerificationToken(){}


    public void InvalidateVerificationToken()
    {
        IsValid = false;
    }

    public bool IsTokenValid(string email)
    {
        return IsValid &&
             DateTime.UtcNow < ExpiryDate
             && Email == email;
    }

    public bool IsTokenExpired()
    {
        return DateTime.UtcNow >= ExpiryDate;
    }

    

}
