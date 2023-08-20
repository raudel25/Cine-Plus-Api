using Cine_Plus_Api.Models;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IPayOrderCommandHandler
{
    Task<int> Handler(CreatePayOrder request);

    Task Handler(PayOrder payOrder);
}

public class PayOrderCommandHandler : IPayOrderCommandHandler
{
    private readonly CinePlusContext _context;

    public PayOrderCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<int> Handler(CreatePayOrder request)
    {
        var payOrder = request.PayOrder();

        this._context.PayOrders.Add(payOrder);
        await this._context.SaveChangesAsync();

        return payOrder.Id;
    }

    public async Task Handler(PayOrder payOrder)
    {
        this._context.PayOrders.Remove(payOrder);
        await this._context.SaveChangesAsync();
    }
}