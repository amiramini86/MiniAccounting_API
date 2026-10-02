using FluentValidation;

namespace MiniAccounting.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommandValidator
    : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Account code is required.")
            .MaximumLength(20)
            .WithMessage("Account code cannot exceed 20 characters.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Account name is required.")
            .MaximumLength(100)
            .WithMessage("Account name cannot exceed 100 characters.");
    }
}