using MiniAccounting.Domain.Exceptions;

namespace MiniAccounting.Domain.Entities;

public class JournalEntry
{
    private readonly List<JournalEntryLine> _lines = new();

    public int Id { get; private set; }

    public DateTime Date { get; private set; }

    public string Description { get; private set; }

    public IReadOnlyCollection<JournalEntryLine> Lines => _lines;

    public JournalEntry(
        DateTime date,
        string description)
    {
        Date = date;
        Description = description;
    }

    public void AddLine(JournalEntryLine line)
    {
        _lines.Add(line);
    }

    public void ValidateBalance()
    {
        if (_lines.Count < 2)
        {
            throw new BusinessRuleException(
                "A journal entry must contain at least two lines.");
        }

        var totalDebit = _lines.Sum(x => x.Debit);
        var totalCredit = _lines.Sum(x => x.Credit);

        if (totalDebit <= 0 || totalCredit <= 0)
        {
            throw new BusinessRuleException(
                "A journal entry must have debit and credit amounts.");
        }

        if (totalDebit != totalCredit)
        {
            throw new BusinessRuleException(
                "Journal entry is not balanced.");
        }
    }
    
}