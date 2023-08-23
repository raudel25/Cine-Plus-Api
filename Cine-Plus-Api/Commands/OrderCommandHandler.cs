using Cine_Plus_Api.Models;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IPayOrderCommandHandler
{
    Task<int> Handler(CreateOrder request);

    Task Handler(Order order);
}

public class OrderCommandHandler : IPayOrderCommandHandler
{
    private readonly CinePlusContext _context;

    public OrderCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<int> Handler(CreateOrder request)
    {
        var order = request.Order();

        this._context.Orders.Add(order);
        await this._context.SaveChangesAsync();

        return order.Id;
    }

    public async Task Handler(Order order)
    {
        this._context.Orders.Remove(order);
        await this._context.SaveChangesAsync();
    }
}