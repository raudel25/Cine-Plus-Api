using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Services;

public interface IMoviePropCommandHandler
{
    public Task UpdateActors(int id);

    public Task UpdateDirectors(int id);

    public Task UpdateGenres(int id);
}

public class MoviePropCommandHandler : IMoviePropCommandHandler
{
    private readonly CinePlusContext _context;

    public MoviePropCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task UpdateActors(int id)
    {
        var actor = await this._context.Actors.Include(actor => actor.Movies)
            .SingleOrDefaultAsync(actor => actor.Id == id);

        if (actor is not null && actor.Movies.Count == 0)
        {
            this._context.Actors.Remove(actor);
            await this._context.SaveChangesAsync();
        }
    }

    public async Task UpdateDirectors(int id)
    {
        var director = await this._context.Directors.Include(director => director.Movies)
            .SingleOrDefaultAsync(director => director.Id == id);

        if (director is not null && director.Movies.Count == 0)
        {
            this._context.Directors.Remove(director);
            await this._context.SaveChangesAsync();
        }
    }

    public async Task UpdateGenres(int id)
    {
        var genre = await this._context.Genres.Include(genre => genre.Movies)
            .SingleOrDefaultAsync(genre => genre.Id == id);

        if (genre is not null && genre.Movies.Count == 0)
        {
            this._context.Genres.Remove(genre);
            await this._context.SaveChangesAsync();
        }
    }
}