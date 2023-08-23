using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Queries;

public interface IPayOrderQueryHandler
{
    Task<Order?> Handler(int id);
}

public class OrderQueryHandler : IPayOrderQueryHandler
{
    private readonly CinePlusContext _context;

    public OrderQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<Order?> Handler(int id)
    {
        return await this._context.Orders.Include(order => order.Seats)
            .SingleOrDefaultAsync(payOrder => payOrder.Id == id);
    }
}