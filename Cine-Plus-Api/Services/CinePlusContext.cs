using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Services;

public class CinePlusContext : DbContext
{
    public DbSet<Actor> Actors { get; set; } = null!;

    public DbSet<Director> Directors { get; set; } = null!;

    public DbSet<Genre> Genres { get; set; } = null!;

    public DbSet<Country> Countries { get; set; } = null!;

    public DbSet<Movie> Movies { get; set; } = null!;

    public DbSet<AvailableSeat> AvailableSeats { get; set; } = null!;

    public DbSet<Cinema> Cinemas { get; set; } = null!;

    public DbSet<Discount> Discounts { get; set; } = null!;

    public DbSet<ShowMovie> ShowMovies { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Employ> Employs { get; set; } = null!;

    public DbSet<Manager> Managers { get; set; } = null!;

    public CinePlusContext(DbContextOptions<CinePlusContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employ>()
            .ToTable("Employs")
            .HasDiscriminator<int>("TypeEmploy")
            .HasValue<Employ>(0)
            .HasValue<Manager>(1);
    }
}