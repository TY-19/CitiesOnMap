using CitiesOnMap.Application.Behaviors;
using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Application.Queries.GetNextCity;
using CitiesOnMap.Application.Services;
using CitiesOnMap.Infrastructure.Data;
using CitiesOnMap.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
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
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IMemoryCache, MemoryCache>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddControllers();

WebApplication app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseSerilogRequestLogging();

app.UseCors("LocalHostPolicy");
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();