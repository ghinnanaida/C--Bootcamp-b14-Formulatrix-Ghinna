using BookJournal.DTOs;
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
        }
    }
}