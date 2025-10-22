using Chirp.Infrastructure;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System;

namespace Chirp.Infrastructure.Tests;

public class ChirpDbContextFactoryTest
{
    [Fact]
    public void CreateDbContext_WithDefaultArguments_ReturnsSqliteConfiguredDbContext()
    {
        string testDBPath = $"{Path.GetTempPath()}/chirp/dbContextTest.db";
        Environment.SetEnvironmentVariable("DB_PATH", testDBPath);
        var factory = new ChirpDbContextFactory();

        string[] args = Array.Empty<string>();

        var context = factory.CreateDbContext(args);

        context.Should().NotBeNull();
        context.Should().BeOfType<ChirpDbContext>();

        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.Sqlite");

        context.Database.GetConnectionString().Should().Be($"Data Source={testDBPath}");
    }
}
