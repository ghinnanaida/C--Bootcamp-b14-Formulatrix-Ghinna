using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Data
{
    public class CinemaDbContext : DbContext
    {
        public CinemaDbContext() { }
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<Studio> Studios { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=CinemaDatabase.db");
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("Movies");
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasOne(e => e.Genre)
                      .WithMany(d => d.Movies)
                      .HasForeignKey(e => e.GenreId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genres");
                entity.HasIndex(e => e.Id).IsUnique();
            });

            modelBuilder.Entity<Studio>(entity =>
            {
                entity.ToTable("Studios");
                entity.HasIndex(e => e.Id).IsUnique();
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedules");
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasOne(e => e.Movie)
                    .WithMany(p => p.ShowTime)
                    .HasForeignKey(s => s.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Studio)
                    .WithMany(p => p.ShowTime)
                    .HasForeignKey(s => s.StudioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Comedy" },
                new Genre { Id = 3, Name = "Drama" },
                new Genre { Id = 4, Name = "Sci-Fi" },
                new Genre { Id = 5, Name = "Horror" },
                new Genre { Id = 6, Name = "Romance" },
                new Genre { Id = 7, Name = "Thriller" },
                new Genre { Id = 8, Name = "Animation" }
            );

            modelBuilder.Entity<Studio>().HasData(
                new Studio { Id = 1, Name = "Studio 1" },
                new Studio { Id = 2, Name = "Studio 2" },
                new Studio { Id = 3, Name = "Studio 3 (IMAX)" },
                new Studio { Id = 4, Name = "Studio 4 (Premiere)" },
                new Studio { Id = 5, Name = "Studio 5" },
                new Studio { Id = 6, Name = "Studio 6" },
                new Studio { Id = 7, Name = "Studio 7" }
            );

            modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1, Title = "Cosmic Voyager", Description = "An epic journey through space.", ReleaseDate = new DateTime(2023, 5, 12), Rating = 8.5, GenreId = 4 }, // Sci-Fi
                new Movie { Id = 2, Title = "The Last Laugh", Description = "A comedian's final show.", ReleaseDate = new DateTime(2022, 11, 1), Rating = 7.8, GenreId = 2 }, // Comedy
                new Movie { Id = 3, Title = "Echoes of Time", Description = "A historian discovers a secret.", ReleaseDate = new DateTime(2024, 2, 20), Rating = 8.9, GenreId = 3 }, // Drama
                new Movie { Id = 4, Title = "Midnight Shadow", Description = "A detective hunts a killer.", ReleaseDate = new DateTime(2023, 9, 5), Rating = 8.2, GenreId = 7 }, // Thriller
                new Movie { Id = 5, Title = "Cybernetic Dawn", Description = "Robots take over the world.", ReleaseDate = new DateTime(2024, 6, 25), Rating = 7.9, GenreId = 1 }, // Action
                new Movie { Id = 6, Title = "Whispering Pines", Description = "Something stirs in the woods.", ReleaseDate = new DateTime(2022, 10, 31), Rating = 7.5, GenreId = 5 }, // Horror
                new Movie { Id = 7, Title = "Parisian Heartbeat", Description = "A love story in Paris.", ReleaseDate = new DateTime(2023, 2, 14), Rating = 8.1, GenreId = 6 }, // Romance
                new Movie { Id = 8, Title = "Giga-Mech Smash", Description = "Giant robots fighting monsters.", ReleaseDate = new DateTime(2022, 7, 19), Rating = 7.1, GenreId = 1 }, // Action
                new Movie { Id = 9, Title = "The Scribbler's Quest", Description = "A magical book comes to life.", ReleaseDate = new DateTime(2024, 1, 15), Rating = 8.6, GenreId = 8 }, // Animation
                new Movie { Id = 10, Title = "Silent Depths", Description = "A submarine crew faces terror.", ReleaseDate = new DateTime(2023, 8, 8), Rating = 7.3, GenreId = 7 }, // Thriller
                new Movie { Id = 11, Title = "Galactic Gladiators", Description = "An intergalactic tournament.", ReleaseDate = new DateTime(2022, 4, 10), Rating = 7.7, GenreId = 4 }, // Sci-Fi
                new Movie { Id = 12, Title = "The Inheritance", Description = "A family uncovers dark secrets.", ReleaseDate = new DateTime(2024, 3, 22), Rating = 8.4, GenreId = 3 }, // Drama
                new Movie { Id = 13, Title = "Love in the Library", Description = "Two book lovers find each other.", ReleaseDate = new DateTime(2022, 5, 30), Rating = 7.9, GenreId = 6 }, // Romance
                new Movie { Id = 14, Title = "Jester's Folly", Description = "A court jester's hilarious mishaps.", ReleaseDate = new DateTime(2023, 4, 1), Rating = 7.2, GenreId = 2 }, // Comedy
                new Movie { Id = 15, Title = "The Void Beckons", Description = "A terrifying presence haunts a spaceship.", ReleaseDate = new DateTime(2024, 9, 13), Rating = 8.0, GenreId = 5 } // Horror
            );

            var today = new DateTime(2025, 8, 12);
            modelBuilder.Entity<Schedule>().HasData(
                new Schedule { Id = 1, ShowTime = today.AddHours(13), Price = 45000m, MovieId = 1, StudioId = 1 },
                new Schedule { Id = 2, ShowTime = today.AddHours(13.5), Price = 45000m, MovieId = 2, StudioId = 2 },
                new Schedule { Id = 3, ShowTime = today.AddHours(15.5), Price = 45000m, MovieId = 1, StudioId = 1 },
                new Schedule { Id = 4, ShowTime = today.AddHours(16), Price = 45000m, MovieId = 4, StudioId = 3 },
                new Schedule { Id = 5, ShowTime = today.AddHours(18.75), Price = 55000m, MovieId = 5, StudioId = 4 },
                new Schedule { Id = 6, ShowTime = today.AddHours(19), Price = 55000m, MovieId = 1, StudioId = 3 },
                new Schedule { Id = 7, ShowTime = today.AddHours(19.25), Price = 55000m, MovieId = 4, StudioId = 5 },
                new Schedule { Id = 8, ShowTime = today.AddHours(21), Price = 55000m, MovieId = 5, StudioId = 4 },
                new Schedule { Id = 9, ShowTime = today.AddHours(21.5), Price = 55000m, MovieId = 6, StudioId = 6 },
                new Schedule { Id = 10, ShowTime = today.AddHours(21.75), Price = 55000m, MovieId = 7, StudioId = 7 },
                new Schedule { Id = 11, ShowTime = today.AddHours(14), Price = 45000m, MovieId = 8, StudioId = 5 },
                new Schedule { Id = 12, ShowTime = today.AddHours(16.5), Price = 45000m, MovieId = 9, StudioId = 6 },
                new Schedule { Id = 13, ShowTime = today.AddHours(19), Price = 55000m, MovieId = 8, StudioId = 5 },

                new Schedule { Id = 14, ShowTime = today.AddDays(1).AddHours(13), Price = 45000m, MovieId = 10, StudioId = 1 },
                new Schedule { Id = 15, ShowTime = today.AddDays(1).AddHours(13.5), Price = 45000m, MovieId = 11, StudioId = 2 },
                new Schedule { Id = 16, ShowTime = today.AddDays(1).AddHours(15.5), Price = 45000m, MovieId = 10, StudioId = 1 },
                new Schedule { Id = 17, ShowTime = today.AddDays(1).AddHours(16), Price = 45000m, MovieId = 12, StudioId = 3 },
                new Schedule { Id = 18, ShowTime = today.AddDays(1).AddHours(18.75), Price = 55000m, MovieId = 13, StudioId = 4 },
                new Schedule { Id = 19, ShowTime = today.AddDays(1).AddHours(19), Price = 55000m, MovieId = 11, StudioId = 3 },
                new Schedule { Id = 20, ShowTime = today.AddDays(1).AddHours(19.25), Price = 55000m, MovieId = 14, StudioId = 5 },
                new Schedule { Id = 21, ShowTime = today.AddDays(1).AddHours(21), Price = 55000m, MovieId = 13, StudioId = 4 },
                new Schedule { Id = 22, ShowTime = today.AddDays(1).AddHours(21.5), Price = 55000m, MovieId = 15, StudioId = 6 },
                new Schedule { Id = 23, ShowTime = today.AddDays(1).AddHours(21.75), Price = 55000m, MovieId = 1, StudioId = 7 },
            
                new Schedule { Id = 24, ShowTime = today.AddDays(2).AddHours(13), Price = 45000m, MovieId = 2, StudioId = 1 },
                new Schedule { Id = 25, ShowTime = today.AddDays(2).AddHours(13.5), Price = 45000m, MovieId = 3, StudioId = 2 },
                new Schedule { Id = 26, ShowTime = today.AddDays(2).AddHours(15.5), Price = 45000m, MovieId = 2, StudioId = 1 },
                new Schedule { Id = 27, ShowTime = today.AddDays(2).AddHours(16), Price = 45000m, MovieId = 6, StudioId = 3 },
                new Schedule { Id = 28, ShowTime = today.AddDays(2).AddHours(18.75), Price = 55000m, MovieId = 7, StudioId = 4 },
                new Schedule { Id = 29, ShowTime = today.AddDays(2).AddHours(19), Price = 55000m, MovieId = 9, StudioId = 3 },
                new Schedule { Id = 30, ShowTime = today.AddDays(2).AddHours(19.25), Price = 55000m, MovieId = 12, StudioId = 5 },
                new Schedule { Id = 31, ShowTime = today.AddDays(2).AddHours(21), Price = 55000m, MovieId = 14, StudioId = 4 },
                new Schedule { Id = 32, ShowTime = today.AddDays(2).AddHours(21.5), Price = 55000m, MovieId = 15, StudioId = 6 },
                new Schedule { Id = 33, ShowTime = today.AddDays(2).AddHours(21.75), Price = 55000m, MovieId = 1, StudioId = 7 },
                new Schedule { Id = 34, ShowTime = today.AddDays(2).AddHours(14), Price = 45000m, MovieId = 11, StudioId = 2 },
                new Schedule { Id = 35, ShowTime = today.AddDays(2).AddHours(16.5), Price = 45000m, MovieId = 5, StudioId = 7 }
            );
        }
    }
}