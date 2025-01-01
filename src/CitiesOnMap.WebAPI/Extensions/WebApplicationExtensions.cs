using CitiesOnMap.Domain.Entities;
using CitiesOnMap.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace CitiesOnMap.WebAPI.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbContextInitializer>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var initializer = new DbContextInitializer(context, userManager, roleManager, configuration, logger);
        await initializer.InitializeAsync();
    }
}