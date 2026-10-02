using MediatR;
using MiniAccounting.Domain.Entities;
using MiniAccounting.Domain.Interfaces;
using MiniAccounting.Application.Common.Exceptions;

namespace MiniAccounting.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler
    : IRequestHandler<CreateAccountCommand, int>
{
    private readonly IAccountRepository _accountRepository;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<int> Handle(
        CreateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var existingAccount =
            await _accountRepository.GetByCodeAsync(request.Code);

        if (existingAccount is not null)
        {
            throw new ConflictException(
                "Account code already exists.");
        }

        var account = new Account(
            request.Code,
            request.Name);

        await _accountRepository.AddAsync(account);

        await _accountRepository.SaveChangesAsync();

        return account.Id;
    }
}