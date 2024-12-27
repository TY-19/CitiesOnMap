using System.Text;
using CitiesOnMap.Application.Behaviors;
using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Application.Queries.GetNextCity;
using CitiesOnMap.Application.Services;
using CitiesOnMap.Infrastructure.Data;
using CitiesOnMap.Infrastructure.Extensions;
using CitiesOnMap.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(o =>
    {
        o.ClientId = builder.Configuration["OAuth:Google:ClientId"] ?? "";
        o.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"] ?? "";
        o.CallbackPath = "/api/signin-google";
    });
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseOpenIddict();
});
builder.Services.AddOpenIddict()
    .AddCore(o =>
    {
        o.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>();
    })
    .AddServer(o =>
    {
        o.SetUserInfoEndpointUris("api/connect/userInfo");
        o.SetTokenEndpointUris("api/connect/token");
        o.SetIntrospectionEndpointUris("api/connect/token/introspect");
        o.SetRevocationEndpointUris("api/connect/token/revoke");
        
        o.RegisterScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.OfflineAccess);
        
        o.UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableUserInfoEndpointPassthrough();
        
        o.AllowPasswordFlow();
        o.AllowRefreshTokenFlow();
        
        o.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
        o.AddSigningKey(new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Security:SigningKey"]
                                       ?? throw new Exception("Signing key is not configured"))))
            .DisableAccessTokenEncryption();
    })
    .AddValidation(o =>
    {
        o.UseLocalServer();
        o.UseAspNetCore();
    });
builder.Services.AddIdentity<User, Role>(o =>
    {
        o.Password.RequireDigit = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddSerilog(o => o.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddCors(o =>
    o.AddPolicy("LocalHostPolicy", cfg =>
    {
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
        cfg.WithOrigins("http://localhost:4200");
    }));
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Cities on map", Version = "v1" });
    o.AddSecurityDefinition(
        "oauth2",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            In = ParameterLocation.Header,
            Name = "Authorization",
            Flows = new OpenApiOAuthFlows
            {
                Password = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("https://localhost:40443/api/Connect/token"),
                }
            }
        }
    );
    o.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                []
            }
        }
    );
});
builder.Services.AddMediatR(o =>
{
    o.RegisterServicesFromAssemblyContaining<GetNextCityRequest>();
    o.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IMemoryCache, MemoryCache>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddControllers();

WebApplication app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseCors("LocalHostPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cities on map v1");
    c.OAuthClientId("angular-app");
});

app.MapControllers();

app.Run();