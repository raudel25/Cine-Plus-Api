using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Services;

public class CinePlusContext: DbContext
{
    public DbSet<Actor> Actors { get; set; } = null!;
    public DbSet<Director> Directors { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;

    public DbSet<Movie> Movies { get; set; } = null!;

    public CinePlusContext(DbContextOptions<CinePlusContext> options) : base(options)
    {
    }
}