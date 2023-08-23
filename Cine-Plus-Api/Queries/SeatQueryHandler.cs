using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Queries;

public interface IAvailableSeatQueryHandler
{
    Task<ICollection<Seat>> Handler();

    Task<Seat?> Handler(int id);
}

public class SeatQueryHandler : IAvailableSeatQueryHandler
{
    private readonly CinePlusContext _context;

    public SeatQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<ICollection<Seat>> Handler()
    {
        return await this._context.Seats.ToListAsync();
    }

    public async Task<Seat?> Handler(int id)
    {
        return await this._context.Seats.Include(seat => seat.ShowMovie.Discounts)
            .SingleOrDefaultAsync(seat => seat.Id == id);
    }
}