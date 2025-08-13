using BookJournal.DTOs;
using BookJournal.Enumerations;
using FluentValidation;

namespace BookJournal.Validators
{
    public class ProgressTrackerCreateValidator : AbstractValidator<ProgressTrackerCreateDTO>
    {
        public ProgressTrackerCreateValidator()
        {
            RuleFor(x => x.BookId).GreaterThan(0);
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.BookType).IsInEnum();
            RuleFor(x => x.ProgressUnit).IsInEnum();
            RuleFor(x => x.TotalValue).GreaterThan(0);
            RuleFor(x => x.Rating).InclusiveBetween(0, 10);

            RuleFor(x => x.ProgressUnit)
                .Must(p => p == ProgressUnit.Pages)
                .When(x => x.BookType == BookType.Paperbook)
                .WithMessage("Paperbooks should be tracked in Pages.");

            RuleFor(x => x.ProgressUnit)
                .Must(p => p == ProgressUnit.Episodes)
                .When(x => x.BookType == BookType.Audiobook)
                .WithMessage("Audiobooks should be tracked in Episodes.");

            RuleFor(x => x.ProgressUnit)
                .Must(p => p == ProgressUnit.Percent)
                .When(x => x.BookType == BookType.Ebook)
                .WithMessage("Ebooks should be tracked in Percent.");

            RuleFor(x => x.TotalValue)
                .LessThanOrEqualTo(100)
                .When(x => x.ProgressUnit == ProgressUnit.Percent)
                .WithMessage("Total Value for Percent must be 100 or less.");
        }
    }
}
