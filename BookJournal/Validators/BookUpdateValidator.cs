using BookJournal.DTOs;
using FluentValidation;

namespace BookJournal.Validators
{
    public class BookUpdateValidator : AbstractValidator<BookUpdateDTO>
    {
        public BookUpdateValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Title is required and must not exceed 200 characters.");

            RuleFor(x => x.Author)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Author is required and must not exceed 100 characters.");

            RuleFor(x => x.GenreIds).NotEmpty().WithMessage("Please select at least one genre.");
        }
    }
}