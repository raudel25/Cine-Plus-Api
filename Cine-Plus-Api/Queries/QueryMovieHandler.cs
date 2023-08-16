using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Queries;

public interface IMovieQueryHandler
{
    Task<IEnumerable<Movie>> Handler();
    Task<Movie?> Handler(string name);

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
    
    public async Task<Movie?> Handler(string name)
    {
        return await this._context.Movies.SingleOrDefaultAsync(movie => movie.Name == name);
    }
}