using System.Net;
using Cine_Plus_Api.Models;
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

    Task<ApiResponse> Handler(int id);
}

public class DiscountCommandHandler : IDiscountCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IDiscountQueryHandler _discountQuery;

    private readonly IShowMovieQueryHandler _showMovieQuery;

    public DiscountCommandHandler(CinePlusContext context, IDiscountQueryHandler discountQuery,
        IShowMovieQueryHandler showMovieQuery)
    {
        this._context = context;
        this._discountQuery = discountQuery;
        this._showMovieQuery = showMovieQuery;
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
        var responseDiscount = await Find(request.Id);
        if (!responseDiscount.Ok) return responseDiscount.ConvertApiResponse();

        var discountEntry = await this._discountQuery.Handler(request.Name);

        if (discountEntry is not null && discountEntry.Id != request.Id)
            return new ApiResponse(HttpStatusCode.BadRequest, "There is already a discount with the same name");

        var discount = request.Discount();

        this._context.Discounts.Update(discount);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    public async Task<ApiResponse> Handler(int id)
    {
        var responseDiscount = await Find(id);
        if (!responseDiscount.Ok) return responseDiscount.ConvertApiResponse();

        var responseShowMovie = await FindInAvailableShowMovie(id);
        if (!responseShowMovie.Ok) return responseShowMovie;

        var discount = responseDiscount.Value!;

        this._context.Discounts.Remove(discount);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    private async Task<ApiResponse<Discount>> Find(int id)
    {
        var discount = await this._context.Discounts.SingleOrDefaultAsync(discount => discount.Id == id);

        return discount is null
            ? new ApiResponse<Discount>(HttpStatusCode.NotFound, "Not found discount")
            : new ApiResponse<Discount>(discount);
    }

    private async Task<ApiResponse> FindInAvailableShowMovie(int id)
    {
        var filter = await this._showMovieQuery.AvailableDiscount(id);

        return filter.Count != 0
            ? new ApiResponse(HttpStatusCode.BadRequest, "There is a show movie available with this discount")
            : new ApiResponse();
    }
}