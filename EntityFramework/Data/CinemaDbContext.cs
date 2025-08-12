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

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Comedy" },
                new Genre { Id = 3, Name = "Drama" }
            );

            modelBuilder.Entity<Studio>().HasData(
                new Studio { Id = 1, Name = "Studio A" },
                new Studio { Id = 2, Name = "Studio B (IMAX)" },
                new Studio { Id = 3, Name = "Studio C" }
            );

            modelBuilder.Entity<Movie>().HasData(
                new Movie 
                { 
                    Id = 1, 
                    Title = "The Dark Knight", 
                    Description = "A masked vigilante fights crime in Gotham City.",
                    ReleaseDate = new DateTime(2008, 7, 18),
                    Rating = 9.0,
                    GenreId = 1 
                },
                new Movie 
                { 
                    Id = 2, 
                    Title = "Forrest Gump",
                    Description = "The story of a man with a low IQ who accomplished great things.",
                    ReleaseDate = new DateTime(1994, 7, 6),
                    Rating = 8.8,
                    GenreId = 3 
                },
                new Movie
                {
                    Id = 3,
                    Title = "Superbad",
                    Description = "Two co-dependent high school seniors are forced to deal with separation anxiety.",
                    ReleaseDate = new DateTime(2007, 8, 17),
                    Rating = 7.6,
                    GenreId = 2 
                }
            );

            modelBuilder.Entity<Schedule>().HasData(
                new Schedule 
                { 
                    Id = 1,
                    ShowTime = DateTime.Parse("2025-08-12 19:00:00"), 
                    Price = 50000m,
                    MovieId = 1, 
                    StudioId = 1 
                },
                new Schedule
                {
                    Id = 2,
                    ShowTime = DateTime.Parse("2025-08-12 21:30:00"), 
                    Price = 65000m,
                    MovieId = 1, 
                    StudioId = 2 
                },
                
                new Schedule
                {
                    Id = 3,
                    ShowTime = DateTime.Parse("2025-08-12 19:15:00"),
                    Price = 45000m,
                    MovieId = 2,
                    StudioId = 3
                },
                new Schedule
                {
                    Id = 4,
                    ShowTime = DateTime.Parse("2025-08-13 20:00:00"), 
                    Price = 45000m,
                    MovieId = 3, 
                    StudioId = 1 
                }
            );
        }
    }
}