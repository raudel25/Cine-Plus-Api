using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IPayOrderCommandHandler
{
    Task<int> Handler(CreatePayOrder request);
}

public class PayOrderCommandHandler:IPayOrderCommandHandler
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
}