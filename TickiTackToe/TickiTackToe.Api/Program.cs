
using Microsoft.EntityFrameworkCore;
using TickiTackToe.Application.Configurations;
using TickiTackToe.Application.Interfaces;
using TickiTackToe.Infrastructure.Data;
using TickiTackToe.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

string connection = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<TickDbContext>(options =>
    options.UseNpgsql(connection)
);

builder.Services.Configure<GameConfig>(builder.Configuration.GetSection(nameof(GameConfig)));

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ILuckyService, LuckyService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TickDbContext>();

    if (app.Environment.IsDevelopment())
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        if (!db.Database.CanConnect())
            throw new Exception("Database not found");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok());

app.Run();
