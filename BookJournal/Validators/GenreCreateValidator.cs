using BookJournal.DTOs;
using FluentValidation;

namespace BookJournal.Validators
{
    public class GenreCreateValidator : AbstractValidator<GenreCreateDTO>
    {
        public GenreCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100).Must(name => name.ToLower() == name.ToLower()).WithMessage("Genre name must be case-insensitively unique.");
        }
    }
}