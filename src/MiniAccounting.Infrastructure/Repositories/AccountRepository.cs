using Microsoft.EntityFrameworkCore;
using MiniAccounting.Domain.Entities;
using MiniAccounting.Domain.Interfaces;
using MiniAccounting.Infrastructure.Persistence.Context;

namespace MiniAccounting.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly MiniAccountingDbContext _context;

    public AccountRepository(MiniAccountingDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Account?> GetByCodeAsync(string code)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Account>> GetByIdsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken)
    {
        var accountIds = ids.Distinct().ToList();

        return await _context.Accounts
            .AsNoTracking()
            .Where(account => accountIds.Contains(account.Id))
            .ToListAsync(cancellationToken);
    }
    
}