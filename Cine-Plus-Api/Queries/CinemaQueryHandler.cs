using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Queries;

public interface ICinemaQueryHandler
{
    Task<IEnumerable<Cinema>> Handler();
}

public class CinemaQueryHandler : ICinemaQueryHandler
{
    private readonly CinePlusContext _context;

    public CinemaQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<IEnumerable<Cinema>> Handler()
    {
        return await this._context.Cinemas.ToListAsync();
    }
}