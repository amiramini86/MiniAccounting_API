using MediatR;
using MiniAccounting.Application.Accounts.DTOs;

namespace MiniAccounting.Application.Accounts.Queries.GetAccountById;

public record GetAccountByIdQuery(
    int Id
) : IRequest<AccountDto?>;