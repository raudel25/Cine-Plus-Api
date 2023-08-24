using System.Net;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Services;

public interface IPaymentService
{
    Task<ResponseGeneratePayOrder> GenerateOrder(GeneratePayOrder request);

    Task<ApiResponse> CancelPayOrder(int id);

    Task<ApiResponse<IEnumerable<ResponsePaidSeat>>> PayCreditCard(int id, PayCreditCard request);
}

public class PaymentService : IPaymentService
{
    private readonly ISeatCommandHandler _seatCommand;

    private readonly ISeatQueryHandler _seatQuery;

    private readonly IOrderCommandHandler _orderCommand;

    private readonly SecurityService _securityService;

    private readonly IOrderQueryHandler _orderQuery;

    private readonly CheckOrderService _checkOrderService;

    private readonly IPayCommandHandler _payCommand;

    public const string Payment = "payment";

    public const string CancelPayment = "cancel_payment";

    public PaymentService(ISeatCommandHandler seatCommand,
        ISeatQueryHandler seatQuery, SecurityService securityService,
        IOrderCommandHandler orderCommand, IOrderQueryHandler orderQuery,
        CheckOrderService checkOrderService, IPayCommandHandler payCommand)
    {
        this._seatCommand = seatCommand;
        this._seatQuery = seatQuery;
        this._securityService = securityService;
        this._orderCommand = orderCommand;
        this._orderQuery = orderQuery;
        this._checkOrderService = checkOrderService;
        this._payCommand = payCommand;
    }

    public async Task<ResponseGeneratePayOrder> GenerateOrder(GeneratePayOrder request)
    {
        var (validSeats, seatsResponse, price) = await ProcessSeats(request);

        var responsePay = new ResponseGeneratePayOrder { Seats = seatsResponse, Price = price };

        if (validSeats.Count == 0) return responsePay;

        var createPayOrder = new CreateOrder { Seats = validSeats, Price = price };
        var id = await this._orderCommand.Create(createPayOrder);

        var token = this._securityService.JwtPay(id, Payment, price, DateTime.UtcNow.AddMinutes(10));
        responsePay.Token = token;

        var now = DateTime.UtcNow;
        responsePay.Date = ((DateTimeOffset)now).ToUnixTimeSeconds();

        this._checkOrderService.Add(id.ToString(), id, TimeSpan.FromMinutes(10));

        return responsePay;
    }

    public async Task<ApiResponse<IEnumerable<ResponsePaidSeat>>> PayCreditCard(int id, PayCreditCard request)
    {
        var response = await CheckFindOrder(id);
        if (!response.Ok) return response.ConvertApiResponse<IEnumerable<ResponsePaidSeat>>();
        var order = response.Value!;

        if (Math.Abs(order.Price - request.Amount) > 0.009)
            return new ApiResponse<IEnumerable<ResponsePaidSeat>>(HttpStatusCode.BadRequest, "The money paid is wrong");

        await this._orderCommand.Pay(order);
        await this._payCommand.CreditCard(id, request);
        await PaidSeats(order);

        return new ApiResponse<IEnumerable<ResponsePaidSeat>>(await ResponsePaidSeats(order));
    }

    public async Task<ApiResponse> CancelPayOrder(int id)
    {
        var response = await CheckFindOrder(id);
        if (!response.Ok) return response.ConvertApiResponse();
        var order = response.Value!;

        foreach (var seat in order.Seats)
        {
            await this._seatCommand.Available(seat.Id);
        }

        await this._orderCommand.Remove(order);

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

    private static double CalculatePrice(Seat seat)
    {
        var d = seat.Discounts.Select(discount => discount.DiscountPercent).Sum();
        return (100 - d) * seat.Price / 100;
    }

    private async Task<ApiResponse<Order>> CheckFindOrder(int id)
    {
        var order = await this._orderQuery.HandlerSeats(id);
        if (order is null) return new ApiResponse<Order>(HttpStatusCode.NotFound, "Not found pay order");

        if (order.Paid) return new ApiResponse<Order>(HttpStatusCode.BadRequest, "The order has been paid");

        this._checkOrderService.Remove(id.ToString());

        return new ApiResponse<Order>(order);
    }

    private async Task<IEnumerable<ResponsePaidSeat>> ResponsePaidSeats(Order order)
    {
        var list = new List<ResponsePaidSeat>();

        foreach (var seat in order.Seats)
        {
            var token = await CancelPaidToken(seat.Id);
            list.Add(new ResponsePaidSeat
            {
                Id = seat.Id, Token = token,
                Description = token is null
                    ? "This pay has not been canceled"
                    : "This payment can be canceled up to two hours before of the show movie"
            });
        }

        return list;
    }

    private async Task<string?> CancelPaidToken(int id)
    {
        var seatDiscounts = await this._seatQuery.HandlerDiscounts(id);

        var seatShowMovie = await this._seatQuery.HandlerShowMovie(id);

        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seatShowMovie!.ShowMovie.Date);
        var date = dateTimeOffset.DateTime;

        var expire = date.Subtract(TimeSpan.FromHours(2));

        var now = DateTime.UtcNow;
        var possible = now <= expire;

        return possible
            ? this._securityService.JwtPay(id, CancelPayment, CalculatePrice(seatDiscounts!),
                expire)
            : null;
    }

    private async Task PaidSeats(Order order)
    {
        foreach (var seat in order.Seats)
        {
            await this._seatCommand.Bought(seat);
        }
    }
}