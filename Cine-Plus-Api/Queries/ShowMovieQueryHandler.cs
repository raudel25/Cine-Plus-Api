using System.Net;
using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Queries;

public interface IShowMovieQueryHandler
{
    Task<IEnumerable<ShowMovie>> Handler();

    Task<ApiResponse> IsValid(ShowMovie showMovie);

    Task<ICollection<ShowMovie>> AvailableMovie(int id);

    Task<ICollection<ShowMovie>> AvailableDiscount(int id);

    Task<ICollection<ShowMovie>> AvailableCinema(int id);
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


    public async Task<ApiResponse> IsValid(ShowMovie showMovie)
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

    private bool AvailableShowMovie(ShowMovie showMovie)
    {
        var now = DateTime.Now;
        return ((DateTimeOffset)now).ToUnixTimeSeconds() <= showMovie.Date;
    }

    public async Task<ICollection<ShowMovie>> AvailableMovie(int id)
    {
        return await this._context.ShowMovies
            .Where(showMovie => AvailableShowMovie(showMovie) && showMovie.MovieId == id)
            .ToListAsync();
    }

    public async Task<ICollection<ShowMovie>> AvailableDiscount(int id)
    {
        var discount = await this._context.Discounts.SingleOrDefaultAsync(discount => discount.Id == id);

        return discount is null ? new List<ShowMovie>() : discount.ShowMovies.Where(AvailableShowMovie).ToList();
    }

    public async Task<ICollection<ShowMovie>> AvailableCinema(int id)
    {
        return await this._context.ShowMovies
            .Where(showMovie => AvailableShowMovie(showMovie) && showMovie.CinemaId == id)
            .ToListAsync();
    }
}