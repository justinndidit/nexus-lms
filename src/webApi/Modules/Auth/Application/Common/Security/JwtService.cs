using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using webApi.Modules.Auth.Domain.Interfaces;

namespace webApi.Modules.Auth.Application.Common.Security;

public class JwtService : IJwtService
{
    private readonly string _jwtSecret;
    private readonly string _jwtExpiration;
    private readonly string _refreshTokenExpiration;

    public JwtService(IConfiguration config)
    {
        _jwtSecret = config.GetSection("JwtConfig").GetSection("JwtSecret").Value ?? throw new Exception("jwt secret can not be null");
        _refreshTokenExpiration = config.GetSection("JwtConfig").GetSection("RefreshTokenExpirationInMinutes").Value ?? throw new Exception("Jwt expiration cannot be null");

        _jwtExpiration = config.GetSection("JwtConfig").GetSection("JwtExpirationInMinutes").Value ?? throw new Exception("Jwt expiration cannot be null");
    }
    public string GenerateAccessToken(Dtos.JwtPayload payload) =>  _generateToken(payload);
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,      
            ValidateAudience = false,    
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateLifetime = false 
        };

        var principal = tokenHandler.ValidateToken(
            token,
            validationParameters,
            out SecurityToken securityToken
        );

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;

    }

    private string _generateToken(Modules.Auth.Application.Dtos.JwtPayload payload)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);
        var userClaims = new List<Claim>
        {   
            new(ClaimTypes.Email, payload.Email),
            new(ClaimTypes.NameIdentifier, payload.UserId.ToString())
        };

        foreach (var role in payload.Roles)
        {
            userClaims.Add(
                new(ClaimTypes.Role, role)
            );
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userClaims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_jwtExpiration)),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    // public bool ValidateToken(string token)
    // {
    //     throw new NotImplementedException();
    // }

    // private string _extractEmail(string token)
    // {
    //     var claims = _extractClaims(token);

    //     var email = claims?
    //         .FirstOrDefault(c => c.Type == ClaimTypes.Email)
    //         ?.Value;

    //     if (string.IsNullOrWhiteSpace(email))
    //         throw new SecurityTokenException("Invalid token: Email claim missing");

    //     return email;
    // }


    // private IEnumerable<Claim> _extractClaims(string token)
    // {
    //     return new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
    // }


}
