using CitiesOnMap.WebAPI.Extensions;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

WebApplication app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseCors(app.Configuration["Cors:PolicyName"] ?? "Default");
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cities on map v1"));

app.MapControllers();

app.Run();