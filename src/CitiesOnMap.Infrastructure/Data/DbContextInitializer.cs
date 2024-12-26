using CitiesOnMap.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace CitiesOnMap.Infrastructure.Data;

public class DbContextInitializer(
    AppDbContext context,
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOpenIddictScopeManager scopeManager,
    IOpenIddictApplicationManager applicationManager,
    IConfiguration configuration,
    ILogger<DbContextInitializer> logger
)
{
    private readonly string _adminEmail = configuration["DefaultAdmin:Email"] ?? "admin@example.com";
    private readonly string _adminPassword = configuration["DefaultAdmin:Password"] ?? "Pa$$w0rd";

    public async Task InitializeAsync()
    {
        await ApplyMigrationsAsync();
        await SeedDefaultRolesAsync();
        await SeedDefaultAdminAsync();
        await RegisterOpenIdDictApplicationsAsync();
    }

    private async Task ApplyMigrationsAsync()
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

    private async Task SeedDefaultRolesAsync()
    {
        List<string> roles =
        [
            "Administrator",
            "User"
        ];
        List<string> existing = await roleManager.Roles
            .Where(r => r.Name != null)
            .Select(r => r.Name!)
            .ToListAsync();
        IEnumerable<string> rolesToAdd = roles.Except(existing).ToList();
        if (rolesToAdd.Any())
        {
            foreach (string role in rolesToAdd)
            {
                await roleManager.CreateAsync(new Role(role));
                logger.LogInformation("Seeding the role {Role}", role);
            }

            await context.SaveChangesAsync();
        }
    }

    private async Task SeedDefaultAdminAsync()
    {
        if (string.IsNullOrEmpty(_adminEmail)
            || string.IsNullOrEmpty(_adminPassword))
        {
            logger.LogInformation("Default admin information was not provided. " +
                                  "Default admin was not seeded.");
            return;
        }

        if (await userManager.FindByEmailAsync(_adminEmail) != null)
        {
            logger.LogInformation("User with the email '{AdminEmail}' already exists.", _adminEmail);
            return;
        }

        var administrator = new User { UserName = _adminEmail, Email = _adminEmail };
        await userManager.CreateAsync(administrator, _adminPassword);
        await userManager.AddToRoleAsync(administrator, "Administrator");
        logger.LogInformation("Default admin '{AdminEmail}' has been seeded.", _adminEmail);
    }

    private async Task RegisterOpenIdDictApplicationsAsync()
    {
        var scopeDescriptor = new OpenIddictScopeDescriptor
        {
            Name = "demo_api",
            Resources = { "demo_api" }
        };

        object? scopeInstance = await scopeManager.FindByNameAsync(scopeDescriptor.Name);
        if (scopeInstance == null)
        {
            await scopeManager.CreateAsync(scopeDescriptor);
        }
        else
        {
            await scopeManager.UpdateAsync(scopeInstance, scopeDescriptor);
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "angular-app",
            ClientType = OpenIddictConstants.ClientTypes.Public,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.Endpoints.Revocation,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Token,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Prefixes.Scope + "demo_api",
                OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Permissions.Prefixes.Scope + OpenIddictConstants.Scopes.OfflineAccess
            },
            RedirectUris =
            {
                new Uri("http://localhost:4200"),
                new Uri("https://localhost:40443/swagger/v1/swagger.json")
            }
        };
        object? client = await applicationManager.FindByClientIdAsync("angular-app");
        if (client is not null)
        {
            await applicationManager.DeleteAsync(client);
        }

        await applicationManager.CreateAsync(descriptor);
    }
}