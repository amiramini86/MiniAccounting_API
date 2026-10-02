using MediatR;

namespace MiniAccounting.Application.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(
    string Code,
    string Name
) : IRequest<int>;