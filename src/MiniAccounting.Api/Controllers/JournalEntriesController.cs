using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniAccounting.Application.Common.Responses;
using MiniAccounting.Application.JournalEntries.Commands.CreateJournalEntry;
using MiniAccounting.Application.JournalEntries.DTOs;
using MiniAccounting.Application.JournalEntries.Queries.GetJournalEntryById;
using MiniAccounting.Application.Common.Responses;

namespace MiniAccounting.Api.Controllers;

[ApiController]
[Route("api/journal-entries")]
public class JournalEntriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public JournalEntriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateJournalEntryCommand command)
    {
        var id = await _mediator.Send(command);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<object>.SuccessResponse(new { id }));
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetJournalEntryByIdQuery(id);

        var journalEntry = await _mediator.Send(query);

        return Ok(ApiResponse<JournalEntryDto>.SuccessResponse(journalEntry));
    }
}