using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Queries;

public interface IAvailableSeatQueryHandler
{
    Task<ICollection<AvailableSeat>> Handler();

    Task<AvailableSeat?> Handler(int id);
}

public class AvailableSeatQueryHandler : IAvailableSeatQueryHandler
{
    private readonly CinePlusContext _context;

    public AvailableSeatQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<ICollection<AvailableSeat>> Handler()
    {
        return await this._context.AvailableSeats.ToListAsync();
    }

    public async Task<AvailableSeat?> Handler(int id)
    {
        return await this._context.AvailableSeats.Include(seat => seat.ShowMovie.Discounts)
            .SingleOrDefaultAsync(seat => seat.Id == id);
    }
}