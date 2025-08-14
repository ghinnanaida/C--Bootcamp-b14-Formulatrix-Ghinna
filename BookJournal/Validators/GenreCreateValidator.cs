using BookJournal.DTOs;
using FluentValidation;

namespace BookJournal.Validators
{
    public class GenreCreateValidator : AbstractValidator<GenreCreateDTO>
    {
        public GenreCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Genre name is required and must not exceed 100 characters.");
        }
    }
}