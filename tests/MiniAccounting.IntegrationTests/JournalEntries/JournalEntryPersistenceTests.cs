using Microsoft.EntityFrameworkCore;
using MiniAccounting.Domain.Entities;
using MiniAccounting.Infrastructure.Persistence.Context;

namespace MiniAccounting.IntegrationTests.JournalEntries;

public class JournalEntryPersistenceTests
{
    [Fact]
    public async Task SaveChanges_WhenJournalEntryHasLines_SavesEntryAndLines()
    {
        // Arrange
        var connectionString =
            "Server=localhost,1433;" +
            "Database=MiniAccountingIntegrationTestsDb;" +
            "User Id=sa;" +
            "Password=Cargo@123456;" +
            "TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<MiniAccountingDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var context = new MiniAccountingDbContext(options);

        await context.Database.EnsureCreatedAsync();

        var suffix = Guid.NewGuid().ToString("N")[..8];

        var account1 = new Account($"T{suffix}1", "Test Cash");
        var account2 = new Account($"T{suffix}2", "Test Capital");

        context.Accounts.AddRange(account1, account2);
        await context.SaveChangesAsync();

        var entry = new JournalEntry(
            DateTime.UtcNow,
            "Integration test entry");

        entry.AddLine(new JournalEntryLine(account1.Id, 1000, 0));
        entry.AddLine(new JournalEntryLine(account2.Id, 0, 1000));

        entry.ValidateBalance();

        // Act
        context.JournalEntries.Add(entry);
        await context.SaveChangesAsync();

        // Assert
        var savedEntry = await context.JournalEntries
            .Include(x => x.Lines)
            .SingleAsync(x => x.Id == entry.Id);

        Assert.Equal(2, savedEntry.Lines.Count);
        Assert.Equal(1000, savedEntry.Lines.Sum(x => x.Debit));
        Assert.Equal(1000, savedEntry.Lines.Sum(x => x.Credit));
    }
}