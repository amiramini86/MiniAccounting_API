namespace MiniAccounting.Application.Accounts.DTOs;

public record AccountDto(
    int Id,
    string Code,
    string Name
);