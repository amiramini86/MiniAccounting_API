using MediatR;
using MiniAccounting.Application.Common.Exceptions;
using MiniAccounting.Application.JournalEntries.DTOs;
using MiniAccounting.Domain.Interfaces;

namespace MiniAccounting.Application.JournalEntries.Queries.GetJournalEntryById;

public class GetJournalEntryByIdQueryHandler
    : IRequestHandler<GetJournalEntryByIdQuery, JournalEntryDto>
{
    private readonly IJournalEntryRepository _journalEntryRepository;

    public GetJournalEntryByIdQueryHandler(
        IJournalEntryRepository journalEntryRepository)
    {
        _journalEntryRepository = journalEntryRepository;
    }

    public async Task<JournalEntryDto> Handle(
        GetJournalEntryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var journalEntry = await _journalEntryRepository
            .GetByIdAsync(request.Id);

        if (journalEntry is null)
            throw new NotFoundException("Journal entry not found.");

        var lines = journalEntry.Lines
            .Select(line => new JournalEntryLineDto(
                line.AccountId,
                line.Account.Code,
                line.Account.Name,
                line.Debit,
                line.Credit))
            .ToList();

        return new JournalEntryDto(
            journalEntry.Id,
            journalEntry.Date,
            journalEntry.Description,
            lines.Sum(x => x.Debit),
            lines.Sum(x => x.Credit),
            lines);
    }
}