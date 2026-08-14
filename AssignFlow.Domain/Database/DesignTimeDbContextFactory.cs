using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace AssignFlow.Domain.Database;

/// <summary>
/// Creates the database context for Entity Framework tooling by using the same
/// connection-string precedence as the API host.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Builds a design-time context without embedding environment-specific credentials.
    /// </summary>
    public AppDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";
        var configurationDirectory = ResolveConfigurationDirectory();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? ReadConnectionString(Path.Combine(configurationDirectory, "appsettings.Local.json"))
            ?? ReadConnectionString(Path.Combine(configurationDirectory, $"appsettings.{environmentName}.json"))
            ?? ReadConnectionString(Path.Combine(configurationDirectory, "appsettings.json"))
            ?? throw new InvalidOperationException("The DefaultConnection connection string is not configured.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    private static string ResolveConfigurationDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var apiDirectory = Path.Combine(currentDirectory, "AssignFlow.API");

        return File.Exists(Path.Combine(apiDirectory, "appsettings.json"))
            ? apiDirectory
            : currentDirectory;
    }

    private static string? ReadConnectionString(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(filePath));
        return document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
            && connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection)
                ? defaultConnection.GetString()
                : null;
    }
}
