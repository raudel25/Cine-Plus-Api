using System.Net;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Commands;

public interface IAvailableSeatCommandHandler
{
    Task Create(ShowMovie showMovie, double price);

    Task Update();

    Task<ApiResponse> Reserve(AvailableSeat seat);

    Task Remove(IEnumerable<AvailableSeat> seats);
}

public class AvailableSeatCommandHandler : IAvailableSeatCommandHandler
{
    private readonly CinePlusContext _context;

    public AvailableSeatCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task Create(ShowMovie showMovie, double price)
    {
        for (var i = 1; i <= showMovie.Cinema.CantSeats; i++)
        {
            this._context.AvailableSeats.Add(
                new AvailableSeat { ShowMovieId = showMovie.Id, Number = i, Price = price, Available = true });
        }

        await this._context.SaveChangesAsync();
    }

    public async Task Update()
    {
        var available = await this._context.AvailableSeats.ToListAsync();

        var notAvailable = available.Where(seat => !ShowMovieQueryHandler.AvailableShowMovie(seat.ShowMovie));

        await Remove(notAvailable);
    }

    public async Task<ApiResponse> Reserve(AvailableSeat seat)
    {
        if (!seat.Available) return new ApiResponse(HttpStatusCode.BadRequest, "The seat has been reserved");

        try
        {
            seat.Available = true;
            this._context.AvailableSeats.Update(seat);
            await this._context.SaveChangesAsync();

            return new ApiResponse();
        }
        catch (DbUpdateConcurrencyException)
        {
            return new ApiResponse(HttpStatusCode.BadRequest, "The seat has been reserved");
        }
    }

    public async Task Remove(IEnumerable<AvailableSeat> seats)
    {
        foreach (var seat in seats)
        {
            this._context.Remove(seat);
        }

        await this._context.SaveChangesAsync();
    }
}