using Cine_Plus_Api.CommandsRequest;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Queries;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Commands;

public interface IMovieCommandHandler
{
    Task<int> Handler(CreateMovie request);

    Task Handler(UpdateMovie request);

    Task Handler(int id);
}

public class MovieCommandHandler : IMovieCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IMoviePropQueryHandler _moviePropQuery;

    private readonly IMoviePropCommandHandler _moviePropCommand;

    public MovieCommandHandler(CinePlusContext context, IMoviePropQueryHandler moviePropQuery,
        IMoviePropCommandHandler moviePropCommand)
    {
        this._context = context;
        this._moviePropQuery = moviePropQuery;
        this._moviePropCommand = moviePropCommand;
    }

    private async Task CheckExisting(Movie movie)
    {
        var actors = new List<Actor>(movie.Actors.Count);

        foreach (var actor in movie.Actors)
        {
            var checkActor = await this._moviePropQuery.Actor(actor.Name);

            actors.Add(checkActor ?? actor);
        }

        var director = await this._moviePropQuery.Director(movie.Director.Name) ?? movie.Director;
        var genre = await this._moviePropQuery.Genre(movie.Genre.Name) ?? movie.Genre;
        var country = await this._moviePropQuery.Country(movie.Country.Name) ?? movie.Country;

        movie.Actors = actors;
        movie.Director = director;
        movie.Genre = genre;
        movie.Country = country;
    }

    private async Task UpdateMovieProps(IEnumerable<int> actors, int director, int genre, int country)
    {
        foreach (var actor in actors)
        {
            await this._moviePropCommand.UpdateActors(actor);
        }

        await this._moviePropCommand.UpdateDirectors(director);
        await this._moviePropCommand.UpdateGenres(genre);
        await this._moviePropCommand.UpdateCountries(country);
    }

    private async Task<(IEnumerable<int>, int, int, int)> LastMovieProp(int id)
    {
        var movie = await this._context.Movies.Include(movie => movie.Actors)
            .SingleOrDefaultAsync(movie => movie.Id == id);

        var actors = movie!.Actors.Select(actor => actor.Id).ToList();
        var (director, genre, country) = (movie.DirectorId, movie.GenreId, movie.CountryId);

        return (actors, director, genre, country);
    }

    public async Task<int> Handler(CreateMovie request)
    {
        var movie = request.Movie();

        await CheckExisting(movie);

        this._context.Add(movie);
        await this._context.SaveChangesAsync();

        return movie.Id;
    }

    public async Task Handler(UpdateMovie request)
    {
        var movie = request.Movie();

        await CheckExisting(movie);

        var (actors, director, genre, country) = await LastMovieProp(movie.Id);

        this._context.Update(movie);
        await this._context.SaveChangesAsync();

        await UpdateMovieProps(actors, director, genre, country);
    }

    public async Task Handler(int id)
    {
        var movie = await this._context.Movies.Include(movie => movie.Actors)
            .SingleOrDefaultAsync(movie => movie.Id == id);

        if (movie is null) return;

        var (actors, director, genre, country) = await LastMovieProp(movie.Id);

        this._context.Remove(movie);
        await this._context.SaveChangesAsync();

        await UpdateMovieProps(actors, director, genre, country);
    }
}