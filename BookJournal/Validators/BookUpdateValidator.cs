using BookJournal.DTOs;
using FluentValidation;

namespace BookJournal.Validators
{
    public class BookUpdateValidator : AbstractValidator<BookUpdateDTO>
    {
        public BookUpdateValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200).Must(title => title.ToLower() == title.ToLower()).WithMessage("Book title must be case-insensitively unique.");
            RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
            RuleFor(x => x.GenreIds).NotEmpty().WithMessage("Please select at least one genre.");

            RuleFor(x => new { x.Title, x.Author })
                .Must(x => x.Title.ToLower() != x.Author.ToLower())
                .WithMessage("Title and Author cannot be the same.");
        }
    }
}