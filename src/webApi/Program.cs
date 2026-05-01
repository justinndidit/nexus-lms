using Microsoft.EntityFrameworkCore;
using webApi.Data;
using webApi.Data.DataExtensions;
using webApi.Modules.Auth.Application.Common.Security;
using webApi.Modules.Auth.Application.Services;
using webApi.Modules.Auth.Domain.Interfaces;
using webApi.Modules.Auth.Infrastructure.Repositories;
using webApi.Modules.Rbac.Application.Services;
using webApi.Modules.Rbac.Domain.Interfaces;
using webApi.Modules.Rbac.Infrastructure.Repositories;
using webApi.Modules.Users.Application.Services;
using webApi.Modules.Users.Domain.Interfaces;
using webApi.Modules.Users.Infrastructure.Repositories;
namespace webApi;

public class Program
{
    public static void Main(string[] args)
    {
        DotNetEnv.Env.TraversePath().Load();
        var builder = WebApplication.CreateBuilder(args);
        builder.Services
               .AddDbContext<LMSApiApplicationContext>(options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultSqlConnectionString")
               ));   
        builder.Services.AddScoped<IAuthTokenGenerator, AuthTokenGenerator>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRbacService, RbacService>();
        builder.Services.AddControllers();
        builder.Services.AddValidation();

        var app = builder.Build();
        app.MigrateDB();
        // app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGet("/", () => "hello world");
        app.MapControllers();
    
        app.Run();

    }
}