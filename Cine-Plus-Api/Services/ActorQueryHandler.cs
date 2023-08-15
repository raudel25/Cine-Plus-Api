using Cine_Plus_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Services;

public interface IActorQueryHandler
{
    Task<Actor?> Handler(string name);
}

public class ActorQueryHandler : IActorQueryHandler
{
    private readonly CinePlusContext _context;

    public ActorQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<Actor?> Handler(string name)
    {
        return await this._context.Actors.SingleOrDefaultAsync((actor) => actor.Name == name);
    }
}