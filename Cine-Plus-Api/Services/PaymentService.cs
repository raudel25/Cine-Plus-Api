using System.Net;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Services;

public interface IPaymentService
{
    Task<ResponseGeneratePayOrder> GeneratePayOrder(GeneratePayOrder request);

    Task<ApiResponse> CancelPayOrder(int id);
}

public class PaymentService : IPaymentService
{
    private readonly IAvailableSeatCommandHandler _availableSeatCommand;

    private readonly IAvailableSeatQueryHandler _availableSeatQuery;

    private readonly IPayOrderCommandHandler _payOrderCommand;

    private readonly SecurityService _securityService;

    private readonly IPayOrderQueryHandler _payOrderQuery;

    private readonly CheckOrderService _checkOrderService;

    public const string Payment = "payment";

    public const string CancelPayment = "cancel_payment";

    public PaymentService(IAvailableSeatCommandHandler availableSeatCommand,
        IAvailableSeatQueryHandler availableSeatQuery, SecurityService securityService,
        IPayOrderCommandHandler payOrderCommand, IPayOrderQueryHandler payOrderQuery,
        CheckOrderService checkOrderService)
    {
        this._availableSeatCommand = availableSeatCommand;
        this._availableSeatQuery = availableSeatQuery;
        this._securityService = securityService;
        this._payOrderCommand = payOrderCommand;
        this._payOrderQuery = payOrderQuery;
        this._checkOrderService = checkOrderService;
    }

    public async Task<ResponseGeneratePayOrder> GeneratePayOrder(GeneratePayOrder request)
    {
        var (validSeats, seatsResponse, price) = await ProcessSeats(request);

        var responsePay = new ResponseGeneratePayOrder { Seats = seatsResponse, Price = price };

        if (validSeats.Count == 0) return responsePay;

        var createPayOrder = new CreateOrder { Seats = validSeats, Price = price };
        var id = await this._payOrderCommand.Handler(createPayOrder);

        var token = this._securityService.JwtPay(id, Payment, price, DateTime.UtcNow.AddMinutes(10));
        responsePay.Token = token;

        var now = DateTime.UtcNow;
        responsePay.Date = ((DateTimeOffset)now).ToUnixTimeSeconds();

        this._checkOrderService.Add(id.ToString(), id, TimeSpan.FromMinutes(10));

        return responsePay;
    }

    public async Task<ApiResponse> CancelPayOrder(int id)
    {
        this._checkOrderService.Remove(id.ToString());

        var order = await this._payOrderQuery.Handler(id);
        if (order is null) return new ApiResponse(HttpStatusCode.NotFound, "Not found pay order");

        foreach (var seat in order.Seats)
        {
            await this._availableSeatCommand.Available(seat.Id);
        }

        await this._payOrderCommand.Handler(order);

        return new ApiResponse();
    }

    private async Task<(List<Seat>, List<ResponseSeatOrder>, double)> ProcessSeats(GeneratePayOrder request)
    {
        var validSeats = new List<Seat>();

        var seatsResponse = new List<ResponseSeatOrder>();

        double price = 0;

        foreach (var seat in request.Seats)
        {
            var response = await ProcessSeat(seat);

            if (response.Ok)
            {
                validSeats.Add(response.Value!);
                price += CalculatePrice(response.Value!);
            }

            seatsResponse.Add(new ResponseSeatOrder { Id = seat.Id, Message = response.Ok ? "Ok" : response.Message! });
        }

        return (validSeats, seatsResponse, price);
    }

    private async Task<ApiResponse<Seat>> ProcessSeat(GenerateSeatOrder seatOrder)
    {
        if (seatOrder.Discounts.Distinct().ToList().Count != seatOrder.Discounts.Count)
            return new ApiResponse<Seat>(HttpStatusCode.BadRequest, "The show movie contains repeated discounts");

        var seat = await this._availableSeatQuery.Handler(seatOrder.Id);
        if (seat is null) return new ApiResponse<Seat>(HttpStatusCode.NotFound, "Not found seat");

        var discounts = seat.ShowMovie.Discounts;

        foreach (var discountId in seatOrder.Discounts)
        {
            var discount = discounts.SingleOrDefault(discount => discount.Id == discountId);
            if (discount is null)
                return new ApiResponse<Seat>(HttpStatusCode.NotFound, "Not found discount in show movie");
        }

        var response = await this._availableSeatCommand.Reserve(seat, discounts);
        if (!response.Ok) return response.ConvertApiResponse<Seat>();

        return new ApiResponse<Seat>(seat);
    }

    private static double CalculatePrice(Seat seat)
    {
        var d = seat.Discounts.Select(discount => discount.DiscountPercent).Sum();
        return (100 - d) * seat.Price / 100;
    }
}