using System;

namespace webApi.Modules.Auth.Domain.Models;

public class VerificationToken
{
    public Guid Id {set; get;}
    public required string Email{set; get;}
    public required string Token{set; get;}
    public DateTime ExpiryDate{set; get;}
    public bool IsValid{get;set;} = true;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get;set;}

    private VerificationToken(){}

    public void InvalidateVerificationToken(VerificationToken token)
    {
        token.IsValid = false;
    }

    public bool IsTokenValid(VerificationToken token)
    {
        return 
            token.IsValid &&
                    (token.ExpiryDate - token.CreatedAt).TotalMinutes <= 30 ;
    }

}
