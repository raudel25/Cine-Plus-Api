using Cine_Plus_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Services;

public interface IMovieQueryHandler
{
    Task<IEnumerable<Movie>> Handler();
}

public class MovieQueryHandler : IMovieQueryHandler
{
    private readonly CinePlusContext _context;

    public MovieQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<IEnumerable<Movie>> Handler()
    {
        return await this._context.Movies.ToListAsync();
    }
}