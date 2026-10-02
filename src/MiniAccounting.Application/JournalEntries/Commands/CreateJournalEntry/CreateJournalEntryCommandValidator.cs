using FluentValidation;

namespace MiniAccounting.Application.JournalEntries.Commands.CreateJournalEntry;

public class CreateJournalEntryCommandValidator
    : AbstractValidator<CreateJournalEntryCommand>
{
    public CreateJournalEntryCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(250);

        RuleFor(x => x.Lines)
            .NotNull()
            .WithMessage("Journal entry lines are required.")
            .Must(lines => lines is { Count: >= 2 })
            .WithMessage("A journal entry must have at least two lines.");

        RuleForEach(x => x.Lines)
            .ChildRules(line =>
            {
                line.RuleFor(x => x.AccountId)
                    .GreaterThan(0);

                line.RuleFor(x => x.Debit)
                    .GreaterThanOrEqualTo(0);

                line.RuleFor(x => x.Credit)
                    .GreaterThanOrEqualTo(0);

                line.RuleFor(x => x)
                    .Must(x =>
                        (x.Debit > 0 && x.Credit == 0) ||
                        (x.Credit > 0 && x.Debit == 0))
                    .WithMessage(
                        "Each line must have either debit or credit, but not both.");
            });
    }
}