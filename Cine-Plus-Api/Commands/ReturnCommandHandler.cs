using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IReturnCommandHandler
{
    Task ReturnCreditCard(int seatId, int orderId, double amount);

    Task ReturnPointsUser(int seatId, int orderId, int points);
}

public class ReturnCommandHandler
{
    private readonly CinePlusContext _context;

    public ReturnCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task ReturnCreditCard(int seatId, int orderId, double amount)
    {
        var now = DateTime.UtcNow;
        var returnPay = new ReturnCreditCard
            { Date = ((DateTimeOffset)now).ToUnixTimeSeconds(), OrderId = orderId, SeatId = seatId, Amount = amount };

        this._context.ReturnCreditCards.Add(returnPay);
        await this._context.SaveChangesAsync();
    }

    public async Task ReturnPointsUser(int seatId, int orderId, int points)
    {
        var now = DateTime.UtcNow;
        var returnPay = new ReturnPointsUser
            { Date = ((DateTimeOffset)now).ToUnixTimeSeconds(), OrderId = orderId, SeatId = seatId, Points = points };

        this._context.ReturnPointsUsers.Add(returnPay);
        await this._context.SaveChangesAsync();
    }
}