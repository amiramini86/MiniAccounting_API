using MiniAccounting.Domain.Entities;

namespace MiniAccounting.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(int id);

    Task<Account?> GetByCodeAsync(string code);

    Task AddAsync(Account account);

    Task SaveChangesAsync();
    
    Task<List<Account>> GetByIdsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken);
}