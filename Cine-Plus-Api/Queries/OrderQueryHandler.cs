using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Queries;

public interface IOrderQueryHandler
{
    Task<Order?> HandlerSeats(int id);

    Task<Order?> HandlerPays(int id);
}

public class OrderQueryHandler : IOrderQueryHandler
{
    private readonly CinePlusContext _context;

    public OrderQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<Order?> HandlerSeats(int id)
    {
        return await this._context.Orders.Include(order => order.Seats)
            .SingleOrDefaultAsync(payOrder => payOrder.Id == id);
    }

    public async Task<Order?> HandlerPays(int id)
    {
        return await this._context.Orders.Include(order => order.Pays)
            .SingleOrDefaultAsync(payOrder => payOrder.Id == id);
    }
}