using System.Data;
using BookJournal.DTOs;
using BookJournal.Enumerations;
using BookJournal.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookJournal.Validators
{
    public class ProgressTrackerCreateValidator : AbstractValidator<ProgressTrackerCreateDTO>
    {
        private readonly ApplicationDbContext _context;

        public ProgressTrackerCreateValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.BookId).GreaterThan(0);
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.BookType).IsInEnum();
            RuleFor(x => x.ProgressUnit).IsInEnum();
            RuleFor(x => x.TotalValue).GreaterThan(0);
            RuleFor(x => x.CurrentValue)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(x => x.TotalValue)
                .WithMessage("Current Value must be less than or equal to Total Value.");
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
