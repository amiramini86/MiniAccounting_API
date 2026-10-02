namespace MiniAccounting.Domain.Entities;

public class JournalEntryLine
{
    public int Id { get; private set; }

    public int AccountId { get; private set; }
    
    public Account Account { get; private set; } = null!;
    
    public decimal Debit { get; private set; }

    public decimal Credit { get; private set; }

    public JournalEntryLine(
        int accountId,
        decimal debit,
        decimal credit)
    {
        if (accountId <= 0)
            throw new ArgumentException(
                "Account ID must be greater than zero.");

        if (debit < 0 || credit < 0)
            throw new ArgumentException(
                "Debit and credit cannot be negative.");

        if (debit > 0 && credit > 0)
            throw new ArgumentException(
                "A journal line cannot have both debit and credit.");

        if (debit == 0 && credit == 0)
            throw new ArgumentException(
                "A journal line must have debit or credit.");

        AccountId = accountId;
        Debit = debit;
        Credit = credit;
    }
}