using Microsoft.EntityFrameworkCore;
using webApi.Data.Seeders;

namespace webApi.Data.DataExtensions;

public static class Migrate
{
    public static async void MigrateDB(this WebApplication app)
    {
        try
        {            
            using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LMSApiApplicationContext>();
            
            await dbContext.Database.MigrateAsync();

            await RoleSeeder.SeedAsync(dbContext);
        } 
        catch (Exception e)
        {
            Console.WriteLine(e.Message);   
            throw;
        }
    }
}
