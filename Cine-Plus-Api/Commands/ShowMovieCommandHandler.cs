using System.Net;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Commands;

public interface IShowMovieCommandHandler
{
    Task<ApiResponse<int>> Handler(CreateShowMovie request);
}

public class ShowMovieCommandHandler : IShowMovieCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IShowMovieQueryHandler _showMovieQuery;

    private readonly IDiscountQueryHandler _discountQuery;

    private readonly IAvailableSeatsCommandHandler _availableSeatsCommand;

    public ShowMovieCommandHandler(CinePlusContext context, IShowMovieQueryHandler showMovieQuery,
        IDiscountQueryHandler discountQuery, IAvailableSeatsCommandHandler availableSeatsCommand)
    {
        this._context = context;
        this._showMovieQuery = showMovieQuery;
        this._discountQuery = discountQuery;
        this._availableSeatsCommand = availableSeatsCommand;
    }

    public async Task<ApiResponse> AddDiscounts(ShowMovie showMovie, ICollection<int> discounts)
    {
        if (discounts.Count != discounts.Distinct().ToList().Count)
            return new ApiResponse(HttpStatusCode.BadRequest, "The show movie contains repeated discounts");

        var discountEntries = new List<Discount>(discounts.Count);

        foreach (var discount in discounts)
        {
            var d = await this._discountQuery.Handler(discount);

            if (d is null)
                return new ApiResponse(HttpStatusCode.BadRequest, "The discount does not exist");

            discountEntries.Add(d);
        }

        showMovie.Discounts = discountEntries;

        return new ApiResponse();
    }

    public async Task<ApiResponse<int>> Handler(CreateShowMovie request)
    {
        var showMovie = request.ShowMovie();

        var checkConflict = await this._showMovieQuery.IsValid(showMovie);
        if (!checkConflict.Ok) return checkConflict.ConvertApiResponse<int>();

        var addDiscounts = await AddDiscounts(showMovie, request.Discounts);
        if (!addDiscounts.Ok) return addDiscounts.ConvertApiResponse<int>();

        this._context.ShowMovies.Add(showMovie);
        await this._context.SaveChangesAsync();

        await this._availableSeatsCommand.Handler(showMovie, request.Price);

        return new ApiResponse<int>(showMovie.Id);
    }
}