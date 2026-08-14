using AssignFlow.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace AssignFlow.Tests.Support;

internal static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
