using Cine_Plus_Api.Models;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IOrderCommandHandler
{
    Task<Order> Create(CreateOrder request);

    Task Remove(Order order);

    Task Pay(Order order);
}

public class OrderCommandHandler : IOrderCommandHandler
{
    private readonly CinePlusContext _context;

    public OrderCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<Order> Create(CreateOrder request)
    {
        var order = request.Order();

        this._context.Orders.Add(order);
        await this._context.SaveChangesAsync();

        return order;
    }

    public async Task Remove(Order order)
    {
        this._context.Orders.Remove(order);
        await this._context.SaveChangesAsync();
    }

    public async Task Pay(Order order)
    {
        order.Paid = true;
        this._context.Orders.Update(order);
        await this._context.SaveChangesAsync();
    }
}