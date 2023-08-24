using System.Net;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Helpers;

namespace Cine_Plus_Api.Services;

public interface IPaymentService
{
    Task<ApiResponse<IEnumerable<ResponsePaidSeat>>> PayCreditCard(int id, PayCreditCard request);

    Task<ResponseGeneratePayOrder> PayTicket(GeneratePayOrder request, int employId);

    Task<ApiResponse<IEnumerable<ResponsePaidSeat>>> PayCreditCardWithUser(int id, int userId, PayCreditCard request);

    Task<ResponseGeneratePayOrder> PayTicketWithUser(GeneratePayOrder request, int employId, int userId);
}

public class PaymentService : IPaymentService
{
    private readonly ISeatCommandHandler _seatCommand;

    private readonly ISeatQueryHandler _seatQuery;

    private readonly IOrderCommandHandler _orderCommand;

    private readonly SecurityService _securityService;

    private readonly IPayCommandHandler _payCommand;

    private readonly IAuthCommandHandler _authCommand;

    private readonly IAuthQueryHandler _authQuery;

    private readonly IOrderService _orderService;

    public const string CancelPayment = "cancel_payment";

    public PaymentService(ISeatCommandHandler seatCommand,
        ISeatQueryHandler seatQuery, SecurityService securityService,
        IOrderCommandHandler orderCommand, IPayCommandHandler payCommand, IAuthCommandHandler authCommand,
        IAuthQueryHandler authQuery, IOrderService orderService)
    {
        this._seatCommand = seatCommand;
        this._seatQuery = seatQuery;
        this._securityService = securityService;
        this._orderCommand = orderCommand;
        this._payCommand = payCommand;
        this._authCommand = authCommand;
        this._authQuery = authQuery;
        this._orderService = orderService;
    }


    public async Task<ApiResponse<IEnumerable<ResponsePaidSeat>>> PayCreditCard(int id, PayCreditCard request) =>
        await PayCreditCard(id, request, -1);

    public async Task<ResponseGeneratePayOrder> PayTicket(GeneratePayOrder request, int employId) =>
        await PayTicket(request, employId, -1);

    public async Task<ApiResponse<IEnumerable<ResponsePaidSeat>>> PayCreditCardWithUser(int id, int userId,
        PayCreditCard request) => await PayCreditCard(id, request, userId);

    public async Task<ResponseGeneratePayOrder> PayTicketWithUser(GeneratePayOrder request, int employId, int userId) =>
        await PayTicket(request, employId, userId);

    private async Task<ApiResponse<IEnumerable<ResponsePaidSeat>>> PayCreditCard(int id, PayCreditCard request,
        int userId)
    {
        var response = await this._orderService.FindRemoveOrder(id);
        if (!response.Ok) return response.ConvertApiResponse<IEnumerable<ResponsePaidSeat>>();
        var order = response.Value!;

        if (Math.Abs(order.Price - request.Amount) > 0.009)
            return new ApiResponse<IEnumerable<ResponsePaidSeat>>(HttpStatusCode.BadRequest, "The money paid is wrong");

        await this._orderCommand.Pay(order);
        await this._payCommand.CreditCard(id, request);
        await PaidSeats(order);

        if (userId != -1) await AddPointsUser(userId, order.AddPoints);

        return new ApiResponse<IEnumerable<ResponsePaidSeat>>(await ResponsePaidSeats(order));
    }

    private async Task<ResponseGeneratePayOrder> PayTicket(GeneratePayOrder request, int employId, int userId)
    {
        var (responsePay, order) = await this._orderService.GenerateOrder(request);
        if (order is null) return responsePay;

        await this._orderCommand.Pay(order);
        await this._payCommand.Ticket(order.Id, employId);
        await PaidSeats(order);

        if (userId != -1) await AddPointsUser(userId, order.AddPoints);

        return responsePay;
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
            ? this._securityService.JwtPay(id, CancelPayment, Calculate.CalculatePrice(seatDiscounts!),
                Calculate.CalculatePricePoints(seatDiscounts!), seatDiscounts!.AddPoints,
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


    private async Task AddPointsUser(int id, int points)
    {
        var user = await this._authQuery.User(id);
        if (user is null) return;

        await this._authCommand.User(user, user.Points += points);
    }
}