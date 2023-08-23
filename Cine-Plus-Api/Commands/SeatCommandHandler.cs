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

    Task<ApiResponse> Reserve(Seat seat, ICollection<Discount> discounts);

    Task Available(int id);

    Task Remove(IEnumerable<Seat> seats);
}

public class SeatCommandHandler : IAvailableSeatCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IAvailableSeatQueryHandler _availableSeatQuery;

    public SeatCommandHandler(CinePlusContext context, IAvailableSeatQueryHandler availableSeatQuery)
    {
        this._context = context;
        this._availableSeatQuery = availableSeatQuery;
    }

    public async Task Create(ShowMovie showMovie, double price)
    {
        for (var i = 1; i <= showMovie.Cinema.CantSeats; i++)
        {
            this._context.Seats.Add(
                new Seat { ShowMovieId = showMovie.Id, Number = i, Price = price, State = SeatState.Available });
        }

        await this._context.SaveChangesAsync();
    }

    public async Task Update()
    {
        var available = await this._context.Seats.ToListAsync();

        var notAvailable = available.Where(seat => !ShowMovieQueryHandler.AvailableShowMovie(seat.ShowMovie));

        await Remove(notAvailable);
    }

    public async Task<ApiResponse> Reserve(Seat seat, ICollection<Discount> discounts)
    {
        if (seat.State != SeatState.Available)
            return new ApiResponse(HttpStatusCode.BadRequest, "The seat has been reserved or bought");

        try
        {
            seat.State = SeatState.Reserved;
            seat.Discounts = discounts;
            this._context.Seats.Update(seat);
            await this._context.SaveChangesAsync();

            return new ApiResponse();
        }
        catch (DbUpdateConcurrencyException)
        {
            return new ApiResponse(HttpStatusCode.BadRequest, "The seat has been reserved");
        }
    }

    public async Task Remove(IEnumerable<Seat> seats)
    {
        foreach (var seat in seats)
        {
            this._context.Remove(seat);
        }

        await this._context.SaveChangesAsync();
    }

    public async Task Available(int id)
    {
        var seat = await this._availableSeatQuery.Handler(id);
        if (seat is null) return;

        seat.State = SeatState.Available;
        seat.Discounts = new List<Discount>();

        this._context.Seats.Update(seat);
        await this._context.SaveChangesAsync();
    }
}