using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IAvailableSeatsCommandHandler
{
    Task Handler(ShowMovie showMovie, double price);
}

public class AvailableSeatsCommandHandler : IAvailableSeatsCommandHandler
{
    private readonly CinePlusContext _context;

    public AvailableSeatsCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task Handler(ShowMovie showMovie, double price)
    {
        for (var i = 1; i <= showMovie.Cinema.CantSeats; i++)
        {
            this._context.AvailableSeats.Add(
                new AvailableSeat { ShowMovieId = showMovie.Id, Number = i, Price = price });
        }

        await this._context.SaveChangesAsync();
    }
}