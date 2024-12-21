using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CitiesOnMap.Infrastructure.Data;

public class DbContextInitializer(
    AppDbContext context,
    ILogger<DbContextInitializer> logger
)
{
    public async Task InitializeAsync()
    {
        if (context.Database.IsRelational() && (await context.Database.GetPendingMigrationsAsync()).Any())
        {
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured while migrating the database");
            }
        }
    }
}