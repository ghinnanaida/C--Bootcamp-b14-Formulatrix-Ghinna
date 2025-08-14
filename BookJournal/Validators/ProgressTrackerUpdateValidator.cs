using BookJournal.DTOs;
using BookJournal.Enumerations;
using FluentValidation;

namespace BookJournal.Validators
{
    public class ProgressTrackerUpdateValidator : AbstractValidator<ProgressTrackerUpdateDTO>
    {
        public ProgressTrackerUpdateValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.CurrentValue).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CurrentValue).LessThanOrEqualTo(x => x.TotalValue).WithMessage("Current progress cannot exceed total value.");
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.Rating).InclusiveBetween(0, 10);
        }
    }
}