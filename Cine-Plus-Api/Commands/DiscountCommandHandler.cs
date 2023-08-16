using System.Net;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Commands;

public interface IDiscountCommandHandler
{
    Task<ApiResponse<int>> Handler(CreateDiscount request);

    Task<ApiResponse> Handler(UpdateDiscount request);

    Task Handler(int id);
}

public class DiscountCommandHandler : IDiscountCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IDiscountQueryHandler _discountQuery;

    public DiscountCommandHandler(CinePlusContext context, IDiscountQueryHandler discountQuery)
    {
        this._context = context;
        this._discountQuery = discountQuery;
    }

    public async Task<ApiResponse<int>> Handler(CreateDiscount request)
    {
        var discount = request.Discount();

        var discountEntry = await this._discountQuery.Handler(request.Name);

        if (discountEntry is not null)
            return new ApiResponse<int>(HttpStatusCode.BadRequest, "There is already a discount with the same name");

        this._context.Discounts.Add(discount);
        await this._context.SaveChangesAsync();

        return new ApiResponse<int>(discount.Id);
    }

    public async Task<ApiResponse> Handler(UpdateDiscount request)
    {
        var discount = request.Discount();

        var discountEntry = await this._discountQuery.Handler(request.Name);

        if (discountEntry is not null && discountEntry.Id != discount.Id)
            return new ApiResponse(HttpStatusCode.BadRequest, "There is already a discount with the same name");

        this._context.Discounts.Update(discount);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    public async Task Handler(int id)
    {
        var discount = await this._context.Discounts.SingleOrDefaultAsync(discount => discount.Id == id);

        if (discount is null) return;

        this._context.Discounts.Remove(discount);
        await this._context.SaveChangesAsync();
    }
}