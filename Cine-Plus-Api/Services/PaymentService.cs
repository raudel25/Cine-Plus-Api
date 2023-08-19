using System.Net;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Services;

public interface IPaymentService
{
    Task<ResponseCreatePayOrder> CreatePayOrder(CreatePayOrder request);
}

public class PaymentService : IPaymentService
{
    private readonly IAvailableSeatCommandHandler _availableSeatCommand;

    private readonly IAvailableSeatQueryHandler _availableSeatQuery;

    private readonly SecurityService _securityService;

    public PaymentService(IAvailableSeatCommandHandler availableSeatCommand,
        IAvailableSeatQueryHandler availableSeatQuery, SecurityService securityService)
    {
        this._availableSeatCommand = availableSeatCommand;
        this._availableSeatQuery = availableSeatQuery;
        this._securityService = securityService;
    }

    public async Task<ResponseCreatePayOrder> CreatePayOrder(CreatePayOrder request)
    {
        var validSeats = new List<SeatOrder>();

        var seatsResponse = new List<ResponseSeatOrder>();

        foreach (var seat in request.Seats)
        {
            var response = await ProcessSeat(seat);

            if (response.Ok) validSeats.Add(seat);

            seatsResponse.Add(new ResponseSeatOrder { Id = seat.Id, Message = response.Ok ? "Ok" : response.Message! });
        }

        var responsePay = new ResponseCreatePayOrder { Seats = seatsResponse };

        if (validSeats.Count == 0) return responsePay;
        var createPayOrder = new CreatePayOrder { Seats = validSeats };
        var token = this._securityService.Jwt(createPayOrder);

        responsePay.Token = token;

        return responsePay;
    }

    private async Task<ApiResponse> ProcessSeat(SeatOrder seatOrder)
    {
        if (seatOrder.Discounts.Distinct().ToList().Count != seatOrder.Discounts.Count)
            return new ApiResponse(HttpStatusCode.BadRequest, "The show movie contains repeated discounts");

        var seat = await this._availableSeatQuery.Handler(seatOrder.Id);
        if (seat is null) return new ApiResponse(HttpStatusCode.NotFound, "Not found seat");

        var discounts = seat.ShowMovie.Discounts;

        foreach (var discountId in seatOrder.Discounts)
        {
            var discount = discounts.SingleOrDefault(discount => discount.Id == discountId);
            if (discount is null) return new ApiResponse(HttpStatusCode.NotFound, "Not found discount in show movie");
        }

        return await this._availableSeatCommand.Reserve(seat);
    }
}