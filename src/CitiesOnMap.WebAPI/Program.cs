using CitiesOnMap.Application.Behaviors;
using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Application.Queries.GetNextCity;
using CitiesOnMap.Application.Services;
using CitiesOnMap.Infrastructure.Data;
using CitiesOnMap.Infrastructure.Extensions;
using CitiesOnMap.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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
        
    
});
builder.Services.AddMediatR(o =>
{
    o.RegisterServicesFromAssemblyContaining<GetNextCityRequest>();
    o.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
builder.Services.AddHttpClient();
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

app.MapGet("/", (context) =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});
app.MapControllers();

app.Run();