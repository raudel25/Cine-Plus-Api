using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Commands;

public interface ICinemaCommandHandler
{
    Task<int> Handler(CreateCinema request);

    Task Handler(UpdateCinema request);

    Task Handler(int id);
}

public class CinemaCommandHandler : ICinemaCommandHandler
{
    private readonly CinePlusContext _context;

    public CinemaCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<int> Handler(CreateCinema request)
    {
        var cinema = request.Cinema();

        this._context.Cinemas.Add(cinema);
        await this._context.SaveChangesAsync();

        return cinema.Id;
    }

    public async Task Handler(UpdateCinema request)
    {
        var cinema = request.Cinema();

        this._context.Cinemas.Update(cinema);
        await this._context.SaveChangesAsync();
    }

    public async Task Handler(int id)
    {
        var cinema = await this._context.Cinemas.SingleOrDefaultAsync(cinema => cinema.Id == id);

        if (cinema is null) return;

        this._context.Cinemas.Remove(cinema);
        await this._context.SaveChangesAsync();
    }
}