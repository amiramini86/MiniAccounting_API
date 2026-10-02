using MediatR;

namespace MiniAccounting.Application.JournalEntries.Commands.CreateJournalEntry;

public record CreateJournalEntryLineRequest(
    int AccountId,
    decimal Debit,
    decimal Credit);

public record CreateJournalEntryCommand(
    DateTime Date,
    string Description,
    List<CreateJournalEntryLineRequest> Lines
) : IRequest<int>;