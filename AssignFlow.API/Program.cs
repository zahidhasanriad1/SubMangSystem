using AssignFlow.API.DependencyService;
using AssignFlow.API.Middlewares;
using AssignFlow.API.Seeding;
using AssignFlow.Domain.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Optional local settings keep developer credentials outside source control while preserving a simple local workflow.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Console-first logging keeps local, CI, and container execution independent of host-specific event-log permissions.
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

DependencyInjection.Inject(builder.Services, builder.Configuration);
builder.Services.AddCors(options => options.AddPolicy("AngularClient", policy =>
    policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"])
        .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<CustomExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AngularClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", utc = DateTime.UtcNow })).AllowAnonymous();

if (builder.Configuration.GetValue("Database:ApplyMigrationsOnStartup", false))
{
    await using var scope = app.Services.CreateAsyncScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider, builder.Configuration);
}

await app.RunAsync();

public partial class Program;
