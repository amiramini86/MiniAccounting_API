using MiniAccounting.Domain.Entities;

namespace MiniAccounting.Domain.Interfaces;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(int id);
    Task AddAsync(JournalEntry journalEntry);

    Task SaveChangesAsync();
}