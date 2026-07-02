using Chat.App.Api.Infrastructure;
using Chat.App.Application;
using Chat.App.Application.Common;
using Chat.App.Database;
using Chat.App.Database.Contexts;
using Chat.App.Database.Entities;
using Chat.App.JWT;
using Chat.App.Websockets;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                "http://localhost:5012",
                "https://localhost:7292")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
builder.Services.AddScoped<ICurrentUserInitializer>(sp =>
    (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
builder.Services.AddApplication();
builder.Services.AddDatabase(
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not configured."));
builder.Services.AddJwtTokenGeneration(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddWebSockets();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();
app.UseWebSockets();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapChatWebSocket("/ws");

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatAppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

await AdminUserSeeder.SeedAsync(app.Services);

app.Run();
