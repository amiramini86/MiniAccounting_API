namespace MiniAccounting.Application.JournalEntries.DTOs;

public record JournalEntryLineDto(
    int AccountId,
    string AccountCode,
    string AccountName,
    decimal Debit,
    decimal Credit
);