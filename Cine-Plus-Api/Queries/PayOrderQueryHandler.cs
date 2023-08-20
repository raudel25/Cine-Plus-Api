using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Queries;

public interface IPayOrderQueryHandler
{
    Task<PayOrder?> Handler(int id);
}

public class PayOrderQueryHandler : IPayOrderQueryHandler
{
    private readonly CinePlusContext _context;

    public PayOrderQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<PayOrder?> Handler(int id)
    {
        return await this._context.PayOrders.SingleOrDefaultAsync(payOrder => payOrder.Id == id);
    }
}