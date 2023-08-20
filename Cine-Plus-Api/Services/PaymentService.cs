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

    private readonly CacheService _cacheService;

    public static string RolePayment => "payment";

    public static string RoleCancelPayment => "cancel_payment";

    public PaymentService(IAvailableSeatCommandHandler availableSeatCommand,
        IAvailableSeatQueryHandler availableSeatQuery, SecurityService securityService,
        IPayOrderCommandHandler payOrderCommand, IPayOrderQueryHandler payOrderQuery, CacheService cacheService)
    {
        this._availableSeatCommand = availableSeatCommand;
        this._availableSeatQuery = availableSeatQuery;
        this._securityService = securityService;
        this._payOrderCommand = payOrderCommand;
        this._payOrderQuery = payOrderQuery;
        this._cacheService = cacheService;
    }

    public async Task<ResponseGeneratePayOrder> GeneratePayOrder(GeneratePayOrder request)
    {
        var validSeats = new List<PaidSeat>();

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

        var responsePay = new ResponseGeneratePayOrder { Seats = seatsResponse, Price = price };

        if (validSeats.Count == 0) return responsePay;

        var createPayOrder = new CreatePayOrder { Paid = false, PaidSeats = validSeats, Price = price };
        var id = await this._payOrderCommand.Handler(createPayOrder);

        var token = this._securityService.JwtPay(id, RolePayment, DateTime.UtcNow.AddMinutes(10));
        responsePay.Token = token;

        this._cacheService.Add(id.ToString(), id, TimeSpan.FromMinutes(10), CancelPayOrder);

        return responsePay;
    }

    public async Task<ApiResponse> CancelPayOrder(int id)
    {
        this._cacheService.Remove(id.ToString());

        var payOrder = await this._payOrderQuery.Handler(id);
        if (payOrder is null) return new ApiResponse(HttpStatusCode.NotFound, "Not found pay order");

        if (payOrder.Paid) return new ApiResponse(HttpStatusCode.BadRequest, "The pay order has been paid");

        foreach (var seat in payOrder.PaidSeats)
        {
            await this._availableSeatCommand.Available(seat.Id);
        }

        await this._payOrderCommand.Handler(payOrder);

        return new ApiResponse();
    }

    private async Task<ApiResponse<PaidSeat>> ProcessSeat(GenerateSeatOrder seatOrder)
    {
        if (seatOrder.Discounts.Distinct().ToList().Count != seatOrder.Discounts.Count)
            return new ApiResponse<PaidSeat>(HttpStatusCode.BadRequest, "The show movie contains repeated discounts");

        var seat = await this._availableSeatQuery.Handler(seatOrder.Id);
        if (seat is null) return new ApiResponse<PaidSeat>(HttpStatusCode.NotFound, "Not found seat");

        var discounts = seat.ShowMovie.Discounts;

        foreach (var discountId in seatOrder.Discounts)
        {
            var discount = discounts.SingleOrDefault(discount => discount.Id == discountId);
            if (discount is null)
                return new ApiResponse<PaidSeat>(HttpStatusCode.NotFound, "Not found discount in show movie");
        }

        var response = await this._availableSeatCommand.Reserve(seat);
        if (!response.Ok) return response.ConvertApiResponse<PaidSeat>();

        return new ApiResponse<PaidSeat>(new PaidSeat
        {
            Id = seat.Id, Number = seat.Number, ShowMovieId = seat.ShowMovieId, Price = seat.Price,
            Discounts = discounts
        });
    }

    private double CalculatePrice(PaidSeat seat)
    {
        var d = seat.Discounts.Select(discount => discount.DiscountPercent).Sum();
        return (100 - d) * seat.Price / 100;
    }
}