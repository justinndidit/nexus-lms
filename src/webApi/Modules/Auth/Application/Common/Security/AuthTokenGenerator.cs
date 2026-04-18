using System.Security.Cryptography;
using System.Text;
namespace webApi.Modules.Auth.Application.Common.Security;

public class AuthTokenGenerator
{
    private static readonly char[] chars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
    public static string GenerateCode(int length)
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
                result.Append(chars[byteValue % chars.Length]);
            }

            return result.ToString();
        }
    }
}
