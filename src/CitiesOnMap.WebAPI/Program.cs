var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => 
    o.AddPolicy(name: "LocalHostPolicy", cfg =>
    {
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
        cfg.WithOrigins("http://localhost:4200");
    }));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("LocalHostPolicy");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGet("api/test", () => new { response = "succeeded" });

app.Run();
