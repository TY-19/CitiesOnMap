using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Application.Tests.TestHelpers;

public static class TestHelpers
{
    public static TestDbContext GetTestDbContext()
    {
        return new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
    }
}