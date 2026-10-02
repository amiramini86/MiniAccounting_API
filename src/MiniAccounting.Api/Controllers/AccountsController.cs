using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniAccounting.Application.Accounts.Commands.CreateAccount;
using MiniAccounting.Application.Accounts.Queries.GetAccountById;
using MiniAccounting.Application.Accounts.DTOs;
using MiniAccounting.Application.Common.Responses;
using MiniAccounting.Application.Accounts.Queries.GetAccountById;
using MiniAccounting.Application.Common.Exceptions;

namespace MiniAccounting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAccountCommand command)
    {
        var accountId = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = accountId },
            new
            {
                id = accountId
            });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetAccountByIdQuery(id);

        var account = await _mediator.Send(query);

        if (account is null)
        {
            return NotFound();

        }

        return Ok(
            ApiResponse<AccountDto>.SuccessResponse(account)
        );
    }
    
}