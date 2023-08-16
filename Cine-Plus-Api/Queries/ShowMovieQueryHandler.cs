using System.Net;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Queries;

public interface IShowMovieQueryHandler
{
    Task<IEnumerable<ShowMovie>> Handler();

    Task<ApiResponse> Handler(ShowMovie showMovie);
}

public class ShowMovieQueryHandler : IShowMovieQueryHandler
{
    private readonly CinePlusContext _context;

    public ShowMovieQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<IEnumerable<ShowMovie>> Handler()
    {
        return await this._context.ShowMovies.ToListAsync();
    }


    public async Task<ApiResponse> Handler(ShowMovie showMovie)
    {
        var movie = await this._context.Movies.SingleOrDefaultAsync(movie => movie.Id == showMovie.MovieId);
        var cinema = await this._context.Cinemas.SingleOrDefaultAsync(cinema => cinema.Id == showMovie.CinemaId);

        if (movie is null || cinema is null)
            return new ApiResponse(HttpStatusCode.BadRequest, "The movie or cinema does not exist");

        var (start, end) = (showMovie.Date, showMovie.Date + movie.Duration);

        var showMovies = await this._context.ShowMovies.Where(sm => sm.CinemaId == showMovie.Id).ToListAsync();
        showMovies = showMovies.Where(sm => Conflict(start, end, sm.Date, sm.Date + sm.Movie.Duration)).ToList();

        return showMovies.Count == 0
            ? new ApiResponse()
            : new ApiResponse(HttpStatusCode.BadRequest, "The show movie conflicts the others");
    }

    private bool Conflict(long start, long end, long startMovie, long endMovie)
    {
        var interval = TimeSpan.FromMinutes(30).Milliseconds;
        start -= interval;
        end += interval;

        return (start <= startMovie && startMovie <= end) || (end <= startMovie && endMovie <= end);
    }
}