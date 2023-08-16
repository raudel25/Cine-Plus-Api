using Cine_Plus_Api.Models;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Queries;

public interface IAvailableSeatQueryHandler
{
    Task<IEnumerable<AvailableSeat>> Handler();
}

public class AvailableSeatQueryHandler : IAvailableSeatQueryHandler
{
    private readonly CinePlusContext _context;

    public AvailableSeatQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<IEnumerable<AvailableSeat>> Handler()
    {
        return await this._context.AvailableSeats.ToListAsync();
    }
}