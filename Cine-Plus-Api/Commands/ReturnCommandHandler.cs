using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IReturnCommandHandler
{
    Task ReturnCreditCard(int seatId, int orderId, double amount);

    Task ReturnPointsUser(int seatId, int orderId, int points);
}

public class ReturnCommandHandler : IReturnCommandHandler
{
    private readonly CinePlusContext _context;

    public ReturnCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task ReturnCreditCard(int seatId, int orderId, double amount)
    {
        var returnPay = new ReturnCreditCard
            { Date = Date.NowLong(), OrderId = orderId, SeatId = seatId, Amount = amount };

        this._context.ReturnCreditCards.Add(returnPay);
        await this._context.SaveChangesAsync();
    }

    public async Task ReturnPointsUser(int seatId, int orderId, int points)
    {
        var returnPay = new ReturnPointsUser
            { Date = Date.NowLong(), OrderId = orderId, SeatId = seatId, Points = points };

        this._context.ReturnPointsUsers.Add(returnPay);
        await this._context.SaveChangesAsync();
    }
}