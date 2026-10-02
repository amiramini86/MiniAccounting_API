using MiniAccounting.Domain.Entities;
using MiniAccounting.Domain.Interfaces;
using MiniAccounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MiniAccounting.Infrastructure.Repositories;

public class JournalEntryRepository : IJournalEntryRepository
{
    private readonly MiniAccountingDbContext _context;

    public JournalEntryRepository(MiniAccountingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(JournalEntry journalEntry)
    {
        await _context.Set<JournalEntry>().AddAsync(journalEntry);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<JournalEntry?> GetByIdAsync(int id)
    {
        return await _context.JournalEntries
            .AsNoTracking()
            .Include(entry => entry.Lines)
            .ThenInclude(line => line.Account)
            .FirstOrDefaultAsync(entry => entry.Id == id);
    }
    
}