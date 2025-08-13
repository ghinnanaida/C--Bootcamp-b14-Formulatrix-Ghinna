using BookJournal.Models;
using BookJournal.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookJournal.Data
{
    public class BookJournalDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public BookJournalDbContext(DbContextOptions<BookJournalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ProgressTracker> ProgressTrackers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Publisher).HasMaxLength(100);
                entity.Property(b => b.PublishedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genres");
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookGenres",
                    j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
                    j => j.HasOne<Book>().WithMany().HasForeignKey("BookId"),
                    j =>
                    {
                        j.HasKey("BookId", "GenreId");
                        j.ToTable("BookGenres");
                    });
            modelBuilder.Entity<ProgressTracker>(entity =>
            {
                entity.ToTable("ProgressTrackers");
                entity.HasKey(pt => pt.Id);
                entity.HasOne(pt => pt.Book)
                      .WithMany(b => b.ProgressTrackers)
                      .HasForeignKey(pt => pt.BookId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(pt => pt.User)
                      .WithMany(u => u.ProgressTrackers)
                      .HasForeignKey(pt => pt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(pt => pt.Status).IsRequired();
                entity.Property(pt => pt.BookType).IsRequired();
                entity.Property(pt => pt.LastStatusChangeDate).IsRequired().HasColumnType("datetime");
                entity.Property(pt => pt.StartDate).HasColumnType("datetime");
                entity.Property(pt => pt.EndDate).HasColumnType("datetime");
                entity.Property(pt => pt.ProgressUnit).IsRequired().HasMaxLength(50);
                entity.Property(pt => pt.TotalValue).IsRequired();
                entity.Property(pt => pt.CurrentValue).HasDefaultValue(0);
                entity.Property(pt => pt.Rating).HasDefaultValue(0);
            });

            SeedData(modelBuilder);

        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            var genres = new List<Genre>
            {
                new Genre { Id = 1, Name = "Fantasy" },
                new Genre { Id = 2, Name = "Science Fiction" },
                new Genre { Id = 3, Name = "Mystery" },
                new Genre { Id = 4, Name = "Romance" },
                new Genre { Id = 5, Name = "Thriller" }
            };
            modelBuilder.Entity<Genre>().HasData(genres);

            var books = new List<Book>
            {
                new Book { Id = 1, Title = "The Way of Kings", Author = "Brandon Sanderson", Publisher = "Tor Books", PublishedDate = DateTime.Parse("2010-8-31") },
                new Book { Id = 2, Title = "Dune", Author = "Frank Herbert", Publisher = "Chilton Books", PublishedDate = DateTime.Parse("1965-8-1") },
                new Book { Id = 3, Title = "And Then There Were None", Author = "Agatha Christie", Publisher = "Collins Crime Club", PublishedDate = DateTime.Parse("1939-11-6") },
                new Book { Id = 4, Title = "Pride and Prejudice", Author = "Jane Austen", Publisher = "T. Egerton, Whitehall", PublishedDate = DateTime.Parse("1813-1-28") }
            };
            modelBuilder.Entity<Book>().HasData(books);

            modelBuilder.Entity("BookGenres").HasData(
                new { BookId = 1, GenreId = 1 }, 
                new { BookId = 2, GenreId = 2 }, 
                new { BookId = 3, GenreId = 3 }, 
                new { BookId = 3, GenreId = 5 }, 
                new { BookId = 4, GenreId = 4 }  
            );

        }
    }
}