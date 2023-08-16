using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Queries;

public interface IDiscountQueryHandler
{
    Task<IEnumerable<Discount>> Handler();

    Task<Discount?> Handler(int id);

    Task<Discount?> Handler(string name);
}

public class DiscountQueryHandler : IDiscountQueryHandler
{
    private readonly CinePlusContext _context;

    public DiscountQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<IEnumerable<Discount>> Handler()
    {
        return await this._context.Discounts.ToListAsync();
    }

    public async Task<Discount?> Handler(int id)
    {
        return await this._context.Discounts.SingleOrDefaultAsync(discount => discount.Id == id);
    }
    
    public async Task<Discount?> Handler(string name)
    {
        return await this._context.Discounts.SingleOrDefaultAsync(discount => discount.Name == name);
    }
}