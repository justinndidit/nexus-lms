using System.Security.Cryptography;
using System.Text;
using webApi.Modules.Auth.Domain.Interfaces;
namespace webApi.Modules.Auth.Application.Common.Security;
public class AuthTokenGenerator :IAuthTokenGenerator
{
    private readonly char[] _secret;

    public AuthTokenGenerator(IConfiguration config)
    {
        _secret = (config.GetSection("AuthenticationToken").GetSection("Secret").Value ?? throw new Exception("Jwt expiration cannot be null")).ToCharArray();
    }
    public string GenerateCode(int length)
    {
        if (length != 6 && length != 8)
        {
            throw new ArgumentException("Code length must be either 6 or 8.", nameof(length));
        }

        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            var result = new StringBuilder(length);
            foreach (var byteValue in bytes)
            {
                result.Append(_secret[byteValue % _secret.Length]);
            }

            return result.ToString();
        }
    }
}