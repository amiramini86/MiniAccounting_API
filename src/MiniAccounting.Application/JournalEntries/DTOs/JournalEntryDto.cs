namespace MiniAccounting.Application.JournalEntries.DTOs;

public record JournalEntryDto(
    int Id,
    DateTime Date,
    string Description,
    decimal TotalDebit,
    decimal TotalCredit,
    IReadOnlyCollection<JournalEntryLineDto> Lines
);