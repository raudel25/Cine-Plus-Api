using System.Net;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Helpers;

namespace Cine_Plus_Api.Services;

public interface IOrderService
{
    Task<ResponseGeneratePayOrder> GenerateOrderWeb(GeneratePayOrder request);

    Task<(ResponseGeneratePayOrder, Order?)> GenerateOrder(GeneratePayOrder request);

    Task<ApiResponse> CancelOrder(int id);

    Task<ApiResponse<Order>> FindRemoveOrder(int id);
}

public class OrderService : IOrderService
{
    private readonly ISeatCommandHandler _seatCommand;

    private readonly ISeatQueryHandler _seatQuery;

    private readonly IOrderCommandHandler _orderCommand;

    private readonly SecurityService _securityService;

    private readonly IOrderQueryHandler _orderQuery;

    private readonly CheckOrderService _checkOrderService;

    public const string Payment = "payment";

    public OrderService(ISeatCommandHandler seatCommand,
        ISeatQueryHandler seatQuery, SecurityService securityService,
        IOrderCommandHandler orderCommand, IOrderQueryHandler orderQuery,
        CheckOrderService checkOrderService)
    {
        this._seatCommand = seatCommand;
        this._seatQuery = seatQuery;
        this._securityService = securityService;
        this._orderCommand = orderCommand;
        this._orderQuery = orderQuery;
        this._checkOrderService = checkOrderService;
    }

    public async Task<ResponseGeneratePayOrder> GenerateOrderWeb(GeneratePayOrder request)
    {
        var (responsePay, order) = await GenerateOrder(request);
        if (order is null) return responsePay;

        var token = this._securityService.JwtPay(order.Id, Payment, responsePay.Price, responsePay.PricePoints,
            responsePay.AddPoints, DateTime.UtcNow.AddMinutes(10));
        responsePay.Token = token;

        var now = DateTime.UtcNow;
        responsePay.Date = ((DateTimeOffset)now).ToUnixTimeSeconds();

        this._checkOrderService.Add(order.Id.ToString(), order.Id, TimeSpan.FromMinutes(10));

        return responsePay;
    }

    public async Task<(ResponseGeneratePayOrder, Order?)> GenerateOrder(GeneratePayOrder request)
    {
        var (validSeats, seatsResponse) = await ProcessSeats(request);

        var price = validSeats.Select(Calculate.CalculatePrice).Sum();
        var pricePoints = validSeats.Select(Calculate.CalculatePricePoints).Sum();
        var addPoints = validSeats.Select(seat => seat.AddPoints).Sum();

        var responsePay = new ResponseGeneratePayOrder
            { Seats = seatsResponse, Price = price, PricePoints = pricePoints, AddPoints = addPoints };

        if (validSeats.Count == 0) return (responsePay, null);

        var createPayOrder = new CreateOrder
            { Seats = validSeats, Price = price, PricePoints = pricePoints, AddPoints = addPoints };
        var order = await this._orderCommand.Create(createPayOrder);

        return (responsePay, order);
    }

    public async Task<ApiResponse<Order>> FindRemoveOrder(int id)
    {
        var order = await this._orderQuery.HandlerSeats(id);
        if (order is null) return new ApiResponse<Order>(HttpStatusCode.NotFound, "Not found pay order");

        if (order.Paid) return new ApiResponse<Order>(HttpStatusCode.BadRequest, "The order has been paid");

        this._checkOrderService.Remove(id.ToString());

        return new ApiResponse<Order>(order);
    }

    public async Task<ApiResponse> CancelOrder(int id)
    {
        var response = await FindRemoveOrder(id);
        if (!response.Ok) return response.ConvertApiResponse();
        var order = response.Value!;

        foreach (var seat in order.Seats)
        {
            await this._seatCommand.Available(seat.Id);
        }

        await this._orderCommand.Remove(order);

        return new ApiResponse();
    }

    private async Task<(List<Seat>, List<ResponseSeatOrder>)> ProcessSeats(GeneratePayOrder request)
    {
        var validSeats = new List<Seat>();
        var seatsResponse = new List<ResponseSeatOrder>();

        foreach (var seat in request.Seats)
        {
            var response = await ProcessSeat(seat);

            if (response.Ok)
                validSeats.Add(response.Value!);

            seatsResponse.Add(new ResponseSeatOrder { Id = seat.Id, Message = response.Ok ? "Ok" : response.Message! });
        }

        return (validSeats, seatsResponse);
    }

    private async Task<ApiResponse<Seat>> ProcessSeat(GenerateSeatOrder seatOrder)
    {
        if (seatOrder.Discounts.Distinct().ToList().Count != seatOrder.Discounts.Count)
            return new ApiResponse<Seat>(HttpStatusCode.BadRequest, "The show movie contains repeated discounts");

        var seat = await this._seatQuery.HandlerShowMovieDiscounts(seatOrder.Id);
        if (seat is null) return new ApiResponse<Seat>(HttpStatusCode.NotFound, "Not found seat");

        var discounts = seat.ShowMovie.Discounts;

        foreach (var discountId in seatOrder.Discounts)
        {
            var discount = discounts.SingleOrDefault(discount => discount.Id == discountId);
            if (discount is null)
                return new ApiResponse<Seat>(HttpStatusCode.NotFound, "Not found discount in show movie");
        }

        var response = await this._seatCommand.Reserve(seat, discounts);
        if (!response.Ok) return response.ConvertApiResponse<Seat>();

        return new ApiResponse<Seat>(seat);
    }
}