using Cine_Plus_Api.Commands;
using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Queries;

public interface IAvailableSeatQueryHandler
{
    Task<IEnumerable<AvailableSeat>> Handler();

    Task<AvailableSeat?> Handler(int id);
}

public class AvailableSeatQueryHandler : IAvailableSeatQueryHandler
{
    private readonly CinePlusContext _context;

    private readonly IAvailableSeatCommandHandler _availableSeatCommand;

    public AvailableSeatQueryHandler(CinePlusContext context, IAvailableSeatCommandHandler availableSeatCommand)
    {
        this._context = context;
        this._availableSeatCommand = availableSeatCommand;
    }

    public async Task<IEnumerable<AvailableSeat>> Handler()
    {
        await this._availableSeatCommand.Update();
        return await this._context.AvailableSeats.ToListAsync();
    }

    public async Task<AvailableSeat?> Handler(int id)
    {
        return await this._context.AvailableSeats.Include(seat => seat.ShowMovie.Discounts)
            .SingleOrDefaultAsync(seat => seat.Id == id);
    }
}