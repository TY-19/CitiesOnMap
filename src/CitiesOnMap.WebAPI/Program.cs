using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Application.Services;
using CitiesOnMap.Infrastructure.Data;
using CitiesOnMap.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IImportService, ImportService>();
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