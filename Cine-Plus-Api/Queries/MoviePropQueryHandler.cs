using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Queries;

public interface IMoviePropQueryHandler
{
    Task<IEnumerable<Actor>> Actors();
    Task<Actor?> Actor(string name);
    Task<IEnumerable<Director>> Directors();
    Task<Director?> Director(string name);
    Task<IEnumerable<Genre>> Genres();
    Task<Genre?> Genre(string name);
    Task<IEnumerable<Country>> Countries();
    Task<Country?> Country(string name);
}

public class MoviePropQueryHandler : IMoviePropQueryHandler
{
    private readonly CinePlusContext _context;

    public MoviePropQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<IEnumerable<Actor>> Actors()
    {
        return await this._context.Actors.ToListAsync();
    }

    public async Task<Actor?> Actor(string name)
    {
        return await this._context.Actors.SingleOrDefaultAsync(actor => actor.Name == name);
    }

    public async Task<IEnumerable<Director>> Directors()
    {
        return await this._context.Directors.ToListAsync();
    }

    public async Task<Director?> Director(string name)
    {
        return await this._context.Directors.SingleOrDefaultAsync(director => director.Name == name);
    }

    public async Task<IEnumerable<Genre>> Genres()
    {
        return await this._context.Genres.ToListAsync();
    }

    public async Task<Genre?> Genre(string name)
    {
        return await this._context.Genres.SingleOrDefaultAsync(genre => genre.Name == name);
    }

    public async Task<IEnumerable<Country>> Countries()
    {
        return await this._context.Countries.ToListAsync();
    }

    public async Task<Country?> Country(string name)
    {
        return await this._context.Countries.SingleOrDefaultAsync(country => country.Name == name);
    }
}