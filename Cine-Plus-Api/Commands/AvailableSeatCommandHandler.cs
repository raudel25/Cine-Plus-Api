using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Commands;

public interface IAvailableSeatCommandHandler
{
    Task Create(ShowMovie showMovie, double price);

    Task Update();
    
    Task Remove(IEnumerable<AvailableSeat> seats);
}

public class AvailableSeatCommandHandler : IAvailableSeatCommandHandler
{
    private readonly CinePlusContext _context;

    public AvailableSeatCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task Create(ShowMovie showMovie, double price)
    {
        for (var i = 1; i <= showMovie.Cinema.CantSeats; i++)
        {
            this._context.AvailableSeats.Add(
                new AvailableSeat { ShowMovieId = showMovie.Id, Number = i, Price = price, Available = true });
        }

        await this._context.SaveChangesAsync();
    }

    public async Task Update()
    {
        var available = await this._context.AvailableSeats.ToListAsync();

        var notAvailable = available.Where(seat => !ShowMovieQueryHandler.AvailableShowMovie(seat.ShowMovie));

        await Remove(notAvailable);
    }

    public async Task Remove(IEnumerable<AvailableSeat> seats)
    {
        foreach (var seat in seats)
        {
            this._context.Remove(seat);
        }

        await this._context.SaveChangesAsync();
    }
}