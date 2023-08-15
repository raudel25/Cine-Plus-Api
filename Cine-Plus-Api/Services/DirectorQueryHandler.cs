using Cine_Plus_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Services;

public interface IDirectorQueryHandler
{
    Task<Director?> Handler(string name);
}

public class DirectorQueryHandler : IDirectorQueryHandler
{
    private readonly CinePlusContext _context;

    public DirectorQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<Director?> Handler(string name)
    {
        return await this._context.Directors.SingleOrDefaultAsync((actor) => actor.Name == name);
    }
}