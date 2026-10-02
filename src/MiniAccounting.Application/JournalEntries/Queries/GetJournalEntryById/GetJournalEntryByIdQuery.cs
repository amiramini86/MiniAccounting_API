using MediatR;
using MiniAccounting.Application.JournalEntries.DTOs;

namespace MiniAccounting.Application.JournalEntries.Queries.GetJournalEntryById;

public record GetJournalEntryByIdQuery(int Id)
    : IRequest<JournalEntryDto>;