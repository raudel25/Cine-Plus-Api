using System.Net;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Commands;

public interface IMovieCommandHandler
{
    Task<ApiResponse<int>> Handler(CreateMovie request);

    Task<ApiResponse> Handler(UpdateMovie request);

    Task<ApiResponse> Handler(int id);
}

public class MovieCommandHandler : IMovieCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IMoviePropQueryHandler _moviePropQuery;

    private readonly IMoviePropCommandHandler _moviePropCommand;

    private readonly IMovieQueryHandler _movieQuery;

    private readonly IShowMovieQueryHandler _showMovieQuery;

    public MovieCommandHandler(CinePlusContext context, IMoviePropQueryHandler moviePropQuery,
        IMoviePropCommandHandler moviePropCommand, IMovieQueryHandler movieQuery, IShowMovieQueryHandler showMovieQuery)
    {
        this._context = context;
        this._moviePropQuery = moviePropQuery;
        this._moviePropCommand = moviePropCommand;
        this._movieQuery = movieQuery;
        this._showMovieQuery = showMovieQuery;
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

    public async Task<ApiResponse<int>> Handler(CreateMovie request)
    {
        var responseSameName = await CheckSameName(request.Name);
        if (!responseSameName.Ok) return responseSameName.ConvertApiResponse<int>();

        var movie = request.Movie();

        await CheckExisting(movie);

        this._context.Add(movie);
        await this._context.SaveChangesAsync();

        return new ApiResponse<int>(movie.Id);
    }

    public async Task<ApiResponse> Handler(UpdateMovie request)
    {
        var responseMovie = await Find(request.Id);
        if (!responseMovie.Ok) return responseMovie.ConvertApiResponse();

        var responseSameName = await CheckSameName(request.Name, request.Id);
        if (!responseSameName.Ok) return responseSameName;

        var (actors, director, genre, country) = LastMovieProp(responseMovie.Value!);

        var movie = request.Movie();
        await CheckExisting(movie);

        this._context.Update(movie);
        await this._context.SaveChangesAsync();

        await UpdateMovieProps(actors, director, genre, country);

        return new ApiResponse();
    }

    public async Task<ApiResponse> Handler(int id)
    {
        var responseMovie = await Find(id);
        if (!responseMovie.Ok) return responseMovie.ConvertApiResponse();

        var responseShowMovie = await FindInAvailableShowMovie(id);
        if (!responseShowMovie.Ok) return responseShowMovie;

        var movie = responseMovie.Value!;
        var (actors, director, genre, country) = LastMovieProp(movie);

        this._context.Remove(movie);
        await this._context.SaveChangesAsync();

        await UpdateMovieProps(actors, director, genre, country);

        return new ApiResponse();
    }

    private async Task<ApiResponse> CheckSameName(string name, int id = -1)
    {
        var movieEntry = await this._movieQuery.Handler(name);

        if (movieEntry is not null && movieEntry.Id != id)
            return new ApiResponse(HttpStatusCode.BadRequest, "There is already a movie with the same name");

        return new ApiResponse();
    }

    private async Task<ApiResponse<Movie>> Find(int id)
    {
        var movie = await this._context.Movies.Include(movie => movie.Actors)
            .SingleOrDefaultAsync(movie => movie.Id == id);

        return movie is null
            ? new ApiResponse<Movie>(HttpStatusCode.NotFound, "Not found movie")
            : new ApiResponse<Movie>(movie);
    }

    private async Task<ApiResponse> FindInAvailableShowMovie(int id)
    {
        var filter = await this._showMovieQuery.AvailableMovie(id);

        return filter.Count != 0
            ? new ApiResponse(HttpStatusCode.BadRequest, "There is a show movie available with this movie")
            : new ApiResponse();
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

    private (IEnumerable<int>, int, int, int) LastMovieProp(Movie movie)
    {
        var actors = movie.Actors.Select(actor => actor.Id).ToList();
        var (director, genre, country) = (movie.DirectorId, movie.GenreId, movie.CountryId);

        return (actors, director, genre, country);
    }
}