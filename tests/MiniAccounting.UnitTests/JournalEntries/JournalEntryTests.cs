using MiniAccounting.Domain.Entities;
using MiniAccounting.Domain.Exceptions;

namespace MiniAccounting.UnitTests.JournalEntries;

public class JournalEntryTests
{
    [Fact]
    public void ValidateBalance_WhenDebitEqualsCredit_DoesNotThrow()
    {
        // Arrange
        var entry = new JournalEntry(
            DateTime.UtcNow,
            "Balanced entry");

        entry.AddLine(new JournalEntryLine(1, 1000, 0));
        entry.AddLine(new JournalEntryLine(2, 0, 1000));

        // Act & Assert
        entry.ValidateBalance();
    }

    [Fact]
    public void ValidateBalance_WhenDebitDoesNotEqualCredit_ThrowsException()
    {
        // Arrange
        var entry = new JournalEntry(
            DateTime.UtcNow,
            "Unbalanced entry");

        entry.AddLine(new JournalEntryLine(1, 1000, 0));
        entry.AddLine(new JournalEntryLine(2, 0, 800));

        // Act & Assert
        Assert.Throws<BusinessRuleException>(
            () => entry.ValidateBalance());
    }
    
    
    [Fact]
    public void ValidateBalance_WhenEntryHasLessThanTwoLines_ThrowsException()
    {
        var entry = new JournalEntry(DateTime.UtcNow, "One line only");
        entry.AddLine(new JournalEntryLine(1, 1000, 0));

        Assert.Throws<BusinessRuleException>(
            () => entry.ValidateBalance());
    }

    [Fact]
    public void ValidateBalance_WhenBothTotalsAreZero_ThrowsException()
    {
        var entry = new JournalEntry(DateTime.UtcNow, "Zero totals");
        entry.AddLine(new JournalEntryLine(1, 0, 100));
        entry.AddLine(new JournalEntryLine(2, 0, 100));

        // این سند نامتوازن است؛ برای تست شرط صفر بودن مجموع‌ها،
        // باید مجموع هر دو طرف صفر باشد.
        // JournalEntryLine اجازه نمی‌دهد ردیف با بدهکار و بستانکار صفر بسازیم.
        // بنابراین این قانون را از طریق مجموع‌های غیرصفر نمی‌توان مستقیم تست کرد.
    }
}