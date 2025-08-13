using BookJournal.DTOs;
using FluentValidation;

namespace BookJournal.Validators
{
    public class BookCreateValidator : AbstractValidator<BookCreateDTO>
    {
        public BookCreateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
            RuleFor(x => x.GenreIds).NotEmpty().WithMessage("Please select at least one genre.");
        }
    }
}