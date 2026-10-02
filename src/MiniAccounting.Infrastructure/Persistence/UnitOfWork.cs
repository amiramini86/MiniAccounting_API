using MiniAccounting.Domain.Interfaces;
using MiniAccounting.Infrastructure.Persistence.Context;

namespace MiniAccounting.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly MiniAccountingDbContext _context;

    public UnitOfWork(MiniAccountingDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}