using Microsoft.EntityFrameworkCore;
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