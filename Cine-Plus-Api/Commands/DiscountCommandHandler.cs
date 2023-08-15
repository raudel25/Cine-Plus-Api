using Cine_Plus_Api.CommandsRequest;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Commands;

public interface IDiscountCommandHandler
{
    Task<int> Handler(CreateDiscount request);

    Task Handler(UpdateDiscount request);

    Task Handler(int id);
}

public class DiscountCommandHandler : IDiscountCommandHandler
{
    private readonly CinePlusContext _context;

    public DiscountCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<int> Handler(CreateDiscount request)
    {
        var discount = request.Discount();

        this._context.Discounts.Add(discount);
        await this._context.SaveChangesAsync();

        return discount.Id;
    }

    public async Task Handler(UpdateDiscount request)
    {
        var discount = request.Discount();

        this._context.Discounts.Update(discount);
        await this._context.SaveChangesAsync();
    }

    public async Task Handler(int id)
    {
        var discount = await this._context.Discounts.SingleOrDefaultAsync(discount => discount.Id == id);

        if (discount is null) return;

        this._context.Discounts.Remove(discount);
        await this._context.SaveChangesAsync();
    }
}