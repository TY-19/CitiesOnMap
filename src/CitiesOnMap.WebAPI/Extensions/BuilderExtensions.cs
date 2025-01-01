using CitiesOnMap.Application.Behaviors;
using CitiesOnMap.Application.Helpers;
using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Application.Interfaces.Helpers;
using CitiesOnMap.Application.Interfaces.Identity;
using CitiesOnMap.Application.Interfaces.Services;
using CitiesOnMap.Application.Queries.GetNextCity;
using CitiesOnMap.Application.Services;
using CitiesOnMap.Application.Validators;
using CitiesOnMap.Domain.Constants;
using CitiesOnMap.Domain.Entities;
using CitiesOnMap.Infrastructure.Data;
using CitiesOnMap.Infrastructure.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;

namespace CitiesOnMap.WebAPI.Extensions;

public static class BuilderExtensions
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.RegisterDbContext();
        builder.RegisterAuthentication();
        builder.RegisterAuthorization();
        builder.RegisterIdentity();
        builder.RegisterCors();
        builder.RegisterValidators();
        builder.RegisterMediator();
        builder.RegisterSwagger();

        builder.Services.AddSerilog(o => o.ReadFrom.Configuration(builder.Configuration));
        builder.Services.AddControllers();

        builder.RegisterDiServices();
    }

    private static void RegisterDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var configurationHelper = sp.GetRequiredService<IConfigurationHelper>();
            options.UseSqlServer(configurationHelper.GetConfigurationValue("ConnectionStrings:DefaultConnection"));
        });
    }

    private static void RegisterAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);
        builder.Services.ConfigureOptions<AuthenticationOptions>();
        builder.Services.ConfigureOptions<JwtBearerOptions>();
    }

    private static void RegisterAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());
    }

    private static void RegisterIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<User>>(Defaults.DefaultProvider);
        builder.Services.ConfigureOptions<IdentityOptions>();
    }

    private static void RegisterCors(this WebApplicationBuilder builder)
    {
        string policyName = builder.Configuration["Cors:PolicyName"] ?? "Default";
        string[] headers = builder.Configuration["Cors:AllowedHeaders"]?.Split(',') ?? ["*"];
        string[] methods = builder.Configuration["Cors:AllowedMethods"]?.Split(',') ?? ["*"];
        string[] origins = builder.Configuration["Cors:AllowedMethods"]?.Split(',') ?? ["*"];

        builder.Services.AddCors(o =>
            o.AddPolicy(policyName, cfg =>
            {
                if (headers.Contains("*"))
                {
                    cfg.AllowAnyHeader();
                }
                else
                {
                    cfg.WithHeaders(headers);
                }

                if (methods.Contains("*"))
                {
                    cfg.AllowAnyMethod();
                }
                else
                {
                    cfg.WithMethods(methods);
                }

                cfg.WithOrigins(origins);
            }));
    }

    private static void RegisterValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<RegistrationRequestModelValidator>();
        builder.Services.AddFluentValidationAutoValidation();
    }

    private static void RegisterMediator(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(o =>
        {
            o.RegisterServicesFromAssemblyContaining<GetNextCityRequest>();
            o.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
    }

    private static void RegisterSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        builder.Services.ConfigureOptions<SwaggerGenOptions>();
    }

    private static void RegisterDiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        builder.Services.AddScoped<IMemoryCache, MemoryCache>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IImportService, ImportService>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IUserManager, UserManager>();
        builder.Services.AddScoped<JwtSecurityTokenHandler>();
        builder.Services.AddScoped<IConfigurationHelper, ConfigurationHelper>();
        builder.Services.AddScoped<IHashingHelper, HashingHelper>();
    }
}