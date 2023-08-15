using Cine_Plus_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Services;

public interface IGenreQueryHandler
{
    Task<Genre?> Handler(string name);
}

public class GenreQueryHandler : IGenreQueryHandler
{
    private readonly CinePlusContext _context;

    public GenreQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<Genre?> Handler(string name)
    {
        return await this._context.Genres.SingleOrDefaultAsync((actor) => actor.Name == name);
    }
}