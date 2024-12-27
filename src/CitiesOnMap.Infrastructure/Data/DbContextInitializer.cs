using CitiesOnMap.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace CitiesOnMap.Infrastructure.Data;

public class DbContextInitializer
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ILogger<DbContextInitializer> _logger;
    private readonly string _adminEmail;
    private readonly string _adminPassword;
    private readonly bool _skipSeeding;

    public DbContextInitializer(AppDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IOpenIddictApplicationManager applicationManager,
        IConfiguration configuration,
        ILogger<DbContextInitializer> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _applicationManager = applicationManager;
        _logger = logger;
        _adminEmail = configuration["DefaultAdmin:Email"] ?? "admin@example.com";
        _adminPassword = configuration["DefaultAdmin:Password"] ?? "Pa$$w0rd";
        if (!bool.TryParse(configuration["SkipSeeding"] ?? "false", out _skipSeeding))
        {
            _skipSeeding = false;
        }
    }
    
    public async Task InitializeAsync()
    {
        if (_skipSeeding)
        {
            return;
        }
        await ApplyMigrationsAsync();
        await SeedDefaultRolesAsync();
        await SeedDefaultAdminAsync();
        await RegisterOpenIdDictApplicationsAsync();
    }

    private async Task ApplyMigrationsAsync()
    {
        if (_context.Database.IsRelational() && (await _context.Database.GetPendingMigrationsAsync()).Any())
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while migrating the database");
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
        List<string> existing = await _roleManager.Roles
            .Where(r => r.Name != null)
            .Select(r => r.Name!)
            .ToListAsync();
        IEnumerable<string> rolesToAdd = roles.Except(existing).ToList();
        if (rolesToAdd.Any())
        {
            foreach (string role in rolesToAdd)
            {
                await _roleManager.CreateAsync(new Role(role));
                _logger.LogInformation("Seeding the role {Role}", role);
            }

            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedDefaultAdminAsync()
    {
        if (string.IsNullOrEmpty(_adminEmail)
            || string.IsNullOrEmpty(_adminPassword))
        {
            _logger.LogInformation("Default admin information was not provided. " +
                                   "Default admin was not seeded.");
            return;
        }

        if (await _userManager.FindByEmailAsync(_adminEmail) != null)
        {
            _logger.LogInformation("User with the email '{AdminEmail}' already exists.", _adminEmail);
            return;
        }

        var administrator = new User { UserName = _adminEmail, Email = _adminEmail };
        await _userManager.CreateAsync(administrator, _adminPassword);
        await _userManager.AddToRoleAsync(administrator, "Administrator");
        _logger.LogInformation("Default admin '{AdminEmail}' has been seeded.", _adminEmail);
    }

    private async Task RegisterOpenIdDictApplicationsAsync()
    {
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
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Token,
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
        object? client = await _applicationManager.FindByClientIdAsync("angular-app");
        if (client is not null)
        {
            await _applicationManager.DeleteAsync(client);
        }
        
        await _applicationManager.CreateAsync(descriptor);
    }
}