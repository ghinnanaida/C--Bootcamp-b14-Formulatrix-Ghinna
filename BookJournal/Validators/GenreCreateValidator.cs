using BookJournal.DTOs;
using FluentValidation;

namespace BookJournal.Validators
{
    public class GenreCreateValidator : AbstractValidator<GenreCreateDTO>
    {
        public GenreCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }
}