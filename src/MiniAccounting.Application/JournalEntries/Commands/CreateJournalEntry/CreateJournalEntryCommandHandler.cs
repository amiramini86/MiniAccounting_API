using MediatR;
using MiniAccounting.Domain.Entities;
using MiniAccounting.Domain.Interfaces;
using MiniAccounting.Application.Common.Exceptions;
using MiniAccounting.Domain.Exceptions;
using MiniAccounting.Domain.Interfaces;
    
namespace MiniAccounting.Application.JournalEntries.Commands.CreateJournalEntry;

public class CreateJournalEntryCommandHandler
    : IRequestHandler<CreateJournalEntryCommand, int>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IAccountRepository _accountRepository;
    
    private readonly IUnitOfWork _unitOfWork;

    public CreateJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        CreateJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Lines.Count < 2)
        {
            throw new ArgumentException(
                "A journal entry must have at least two lines.");
        }

        var accountIds = request.Lines
            .Select(line => line.AccountId)
            .Distinct()
            .ToList();

        var accounts = await _accountRepository.GetByIdsAsync(
            accountIds,
            cancellationToken);

        var existingAccountIds = accounts
            .Select(account => account.Id)
            .ToHashSet();

        var missingAccountIds = accountIds
            .Where(id => !existingAccountIds.Contains(id))
            .ToList();

        if (missingAccountIds.Count > 0)
        {
            throw new NotFoundException(
                $"Accounts not found: {string.Join(", ", missingAccountIds)}");
        }

        var journalEntry = new JournalEntry(
            request.Date,
            request.Description);

        foreach (var line in request.Lines)
        {
            journalEntry.AddLine(
                new JournalEntryLine(
                    line.AccountId,
                    line.Debit,
                    line.Credit));
        }

        journalEntry.ValidateBalance();

        await _journalEntryRepository.AddAsync(journalEntry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return journalEntry.Id;
    }

}