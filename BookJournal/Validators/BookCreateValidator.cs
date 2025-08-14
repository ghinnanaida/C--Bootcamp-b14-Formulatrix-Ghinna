using BookJournal.DTOs;
using FluentValidation;

namespace BookJournal.Validators
{
    public class BookCreateValidator : AbstractValidator<BookCreateDTO>
    {
        public BookCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Title is required and must not exceed 200 characters.");

            RuleFor(x => x.Author)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Author is required and must not exceed 100 characters.");

            RuleFor(x => new { x.Title, x.Author })
                .Must(x => x.Title.ToLower() != x.Author.ToLower())
                .WithMessage("Title and Author cannot be the same.");
            
            RuleFor(x => x.GenreIds).NotEmpty().WithMessage("Please select at least one genre.");
            
        }
    }
}