using Microsoft.EntityFrameworkCore;
using MiniAccounting.Domain.Entities;

namespace MiniAccounting.Infrastructure.Persistence.Context;

public class MiniAccountingDbContext : DbContext
{
    public MiniAccountingDbContext(
        DbContextOptions<MiniAccountingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<JournalEntry> JournalEntries =>
        Set<JournalEntry>();

    public DbSet<JournalEntryLine> JournalEntryLines =>
        Set<JournalEntryLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JournalEntry>()
            .HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey("JournalEntryId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JournalEntry>()
            .Property(x => x.Description)
            .HasMaxLength(250)
            .IsRequired();

        modelBuilder.Entity<JournalEntryLine>()
            .HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<JournalEntryLine>()
            .Property(x => x.Debit)
            .HasPrecision(18, 2);

        modelBuilder.Entity<JournalEntryLine>()
            .Property(x => x.Credit)
            .HasPrecision(18, 2);
    }
}