using Cine_Plus_Api.CommandsRequest;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Queries;

namespace Cine_Plus_Api.Commands;
public interface IShowMovieCommandHandler
{
    Task<int> Handler(CreateShowMovie request);
}

public class ShowMovieCommandHandler : IShowMovieCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IShowMovieQueryHandler _showMovieQuery;

    public ShowMovieCommandHandler(CinePlusContext context, IShowMovieQueryHandler showMovieQuery)
    {
        this._context = context;
        this._showMovieQuery = showMovieQuery;
    }

    public async Task<int> Handler(CreateShowMovie request)
    {
        var showMovie = request.ShowMovie();

        var possible = this._showMovieQuery.Handler(showMovie);

        //TODO: Check possible

        this._context.ShowMovies.Add(showMovie);
        await this._context.SaveChangesAsync();

        await CreateAvailableSeats(showMovie);

        return showMovie.Id;
    }

    private async Task CreateAvailableSeats(ShowMovie showMovie)
    {
        for (var i = 1; i <= showMovie.Cinema.CantSeats; i++)
        {
            this._context.AvailableSeats.Add(new AvailableSeat { ShowMovieId = showMovie.Id, Number = i });
        }

        await this._context.SaveChangesAsync();
    }
}