using System.Net;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Services;

public interface IReturnService
{
    Task<ApiResponse> ReturnCreditCard(int id);

    Task<ApiResponse> ReturnPointsUser(int id);
}

public class ReturnService : IReturnService
{
    private readonly ISeatQueryHandler _seatQuery;

    private readonly ISeatCommandHandler _seatCommand;

    private readonly IReturnCommandHandler _returnCommand;

    private readonly IOrderQueryHandler _orderQuery;

    private readonly IAuthCommandHandler _authCommand;

    public ReturnService(ISeatQueryHandler seatQuery, ISeatCommandHandler seatCommand,
        IReturnCommandHandler returnCommand, IOrderQueryHandler orderQuery, IAuthCommandHandler authCommand)
    {
        this._seatQuery = seatQuery;
        this._seatCommand = seatCommand;
        this._returnCommand = returnCommand;
        this._orderQuery = orderQuery;
        this._authCommand = authCommand;
    }

    public async Task<ApiResponse> ReturnCreditCard(int id)
    {
        var seat = await this._seatQuery.HandlerDiscounts(id);
        if (seat is null) return new ApiResponse(HttpStatusCode.NotFound, "Not found seat");

        if (seat.State != SeatState.Bought) return new ApiResponse(HttpStatusCode.BadRequest, "Incorrect seat");

        var seatOrder = await this._seatQuery.HandlerOrder(id);
        var orderId = seatOrder!.Orders.ToList()[0].Id;

        var amount = Calculate.CalculatePrice(seat);

        var orderPay = await this._orderQuery.HandlerPays(orderId);
        if (orderPay!.Pays.ToList()[0] is not CreditCard pay)
            return new ApiResponse(HttpStatusCode.BadRequest, "Incorrect seat");

        await this._returnCommand.ReturnCreditCard(id, orderId, amount);
        await ReturnMoney(pay.Card, amount);
        await this._seatCommand.Available(id);

        return new ApiResponse();
    }

    public async Task<ApiResponse> ReturnPointsUser(int id)
    {
        var seat = await this._seatQuery.HandlerDiscounts(id);
        if (seat is null) return new ApiResponse(HttpStatusCode.NotFound, "Not found seat");

        if (seat.State != SeatState.Bought) return new ApiResponse(HttpStatusCode.BadRequest, "Incorrect seat");

        var seatOrder = await this._seatQuery.HandlerOrder(id);
        var orderId = seatOrder!.Orders.ToList()[0].Id;

        var points = seat.PricePoints;

        var orderPay = await this._orderQuery.HandlerPays(orderId);
        if (orderPay!.Pays.ToList()[0] is not PointsUser pay)
            return new ApiResponse(HttpStatusCode.BadRequest, "Incorrect seat");

        await this._returnCommand.ReturnPointsUser(id, orderId, points);
        await AddPointsUser(pay.UserId, points);
        await this._seatCommand.Available(id);

        return new ApiResponse();
    }

    private async Task AddPointsUser(int id, int points)
    {
        await this._authCommand.User(id, points);
    }

    private async Task ReturnMoney(long card, double amount)
    {
        await Task.Delay(new TimeSpan(0, 0, 5));
    }
}