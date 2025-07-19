
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TickiTackToe.Api.Filters;
using TickiTackToe.Api.Middlewares;
using TickiTackToe.Application.Commands;
using TickiTackToe.Application.Configurations;
using TickiTackToe.Application.Interfaces;
using TickiTackToe.Infrastructure.Cache;
using TickiTackToe.Infrastructure.Data;
using TickiTackToe.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<TickDbContext>(options =>
    options.UseNpgsql(connection)
);

builder.Services.Configure<GameConfig>(builder.Configuration.GetSection(nameof(GameConfig)));
builder.Services.AddSingleton<ILuckyService, LuckyService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddMemoryCache();
// builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MakeMoveCommand).Assembly));

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.OperationFilter<IdempotencyHeaderOperationFilter>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TickDbContext>();
    await db.Database.MigrateAsync();
    if (!db.Database.CanConnect())
        throw new Exception("Database not found");
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<IdempotencyMiddleware>();

app.MapControllers();
app.MapGet("/health", () => Results.Ok());

app.Run();
