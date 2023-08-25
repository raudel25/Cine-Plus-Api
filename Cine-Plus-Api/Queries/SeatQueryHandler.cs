using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Queries;

public interface ISeatQueryHandler
{
    Task<IEnumerable<Seat>> Handler();

    Task<IEnumerable<Seat>> AvailableSeat();

    Task<IEnumerable<Seat>> AvailableSeat(int showMovieId);

    Task<Seat?> HandlerDiscounts(int id);

    Task<Seat?> HandlerShowMovie(int id);

    Task<Seat?> HandlerShowMovieDiscounts(int id);
}

public class SeatQueryHandler : ISeatQueryHandler
{
    private readonly CinePlusContext _context;

    public SeatQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<IEnumerable<Seat>> Handler()
    {
        return await this._context.Seats.ToListAsync();
    }

    public async Task<IEnumerable<Seat>> AvailableSeat()
    {
        return await this._context.Seats.Where(seat => seat.State == SeatState.Available).ToListAsync();
    }

    public async Task<IEnumerable<Seat>> AvailableSeat(int showMovieId)
    {
        return await this._context.Seats.Where(seat => seat.State == SeatState.Available && seat.Id == showMovieId)
            .ToListAsync();
    }

    public async Task<Seat?> HandlerShowMovieDiscounts(int id)
    {
        return await this._context.Seats.Include(seat => seat.ShowMovie.Discounts)
            .SingleOrDefaultAsync(seat => seat.Id == id);
    }

    public async Task<Seat?> HandlerDiscounts(int id)
    {
        return await this._context.Seats.Include(seat => seat.ShowMovie).Include(seat => seat.Discounts)
            .SingleOrDefaultAsync(seat => seat.Id == id);
    }

    public async Task<Seat?> HandlerShowMovie(int id)
    {
        return await this._context.Seats.Include(seat => seat.ShowMovie)
            .SingleOrDefaultAsync(seat => seat.Id == id);
    }
}