using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Services;

public interface IMovieCommandHandler
{
    Task<int> Handler(CreateMovie request);

    Task<int> Handler(UpdateMovie request);
}

public class MovieCommandHandler : IMovieCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IGenreQueryHandler _genreQuery;

    private readonly IActorQueryHandler _actorQuery;

    private readonly IDirectorQueryHandler _directorQuery;

    public MovieCommandHandler(CinePlusContext context, IActorQueryHandler actorQuery,
        IDirectorQueryHandler directorQuery, IGenreQueryHandler genreQuery)
    {
        this._context = context;
        this._actorQuery = actorQuery;
        this._directorQuery = directorQuery;
        this._genreQuery = genreQuery;
    }

    private async Task CheckExisting(Movie movie)
    {
        var actors = new List<Actor>(movie.Actors.Count);

        foreach (var actor in movie.Actors)
        {
            var checkActor = await this._actorQuery.Handler(actor.Name);

            actors.Add(checkActor ?? actor);
        }

        var director = await this._directorQuery.Handler(movie.Director.Name) ?? movie.Director;
        var genre = await this._genreQuery.Handler(movie.Genre.Name) ?? movie.Genre;

        movie.Actors = actors;
        movie.Director = director;
        movie.Genre = genre;
    }

    public async Task<int> Handler(CreateMovie request)
    {
        var movie = request.Movie();

        await CheckExisting(movie);

        this._context.Add(movie);
        await this._context.SaveChangesAsync();

        return movie.Id;
    }

    public async Task<int> Handler(UpdateMovie request)
    {
        var movie = request.Movie();

        await CheckExisting(movie);

        this._context.Update(movie);
        await this._context.SaveChangesAsync();

        return movie.Id;
    }
}