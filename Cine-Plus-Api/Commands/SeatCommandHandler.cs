using System.Net;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Commands;

public interface ISeatCommandHandler
{
    Task Create(CreateSeat request);

    Task UpdateSeats();

    Task<ApiResponse> Reserve(Seat seat, ICollection<Discount> discounts);

    Task Available(int id);

    Task Bought(Seat seat);
}

public class SeatCommandHandler : ISeatCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly ISeatQueryHandler _seatQuery;

    public SeatCommandHandler(CinePlusContext context, ISeatQueryHandler seatQuery)
    {
        this._context = context;
        this._seatQuery = seatQuery;
    }

    public async Task Create(CreateSeat request)
    {
        for (var i = 1; i <= request.ShowMovie.Cinema.CantSeats; i++)
        {
            this._context.Seats.Add(request.Seat(i));
        }

        await this._context.SaveChangesAsync();
    }

    public async Task UpdateSeats()
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
        var seat = await this._seatQuery.HandlerDiscounts(id);
        if (seat is null) return;

        seat.State = SeatState.Available;
        seat.Discounts = new List<Discount>();

        this._context.Seats.Update(seat);
        await this._context.SaveChangesAsync();
    }

    public async Task Bought(Seat seat)
    {
        seat.State = SeatState.Bought;

        this._context.Update(seat);
        await this._context.SaveChangesAsync();
    }
}