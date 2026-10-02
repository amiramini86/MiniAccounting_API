using MediatR;
using MiniAccounting.Application.Accounts.DTOs;
using MiniAccounting.Domain.Interfaces;

namespace MiniAccounting.Application.Accounts.Queries.GetAccountById;

public class GetAccountByIdQueryHandler
    : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdQueryHandler(
        IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto?> Handle(
        GetAccountByIdQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository
            .GetByIdAsync(request.Id);

        if (account is null)
        {
            return null;
        }

        return new AccountDto(
            account.Id,
            account.Code,
            account.Name);
    }
}