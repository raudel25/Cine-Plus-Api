using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Queries;

namespace Cine_Plus_Api.Commands;

public interface IShowMovieCommandHandler
{
    Task<int> Handler(CreateShowMovie request);
}

public class ShowMovieCommandHandler : IShowMovieCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IShowMovieQueryHandler _showMovieQuery;

    private readonly IDiscountQueryHandler _discountQuery;

    public ShowMovieCommandHandler(CinePlusContext context, IShowMovieQueryHandler showMovieQuery,
        IDiscountQueryHandler discountQuery)
    {
        this._context = context;
        this._showMovieQuery = showMovieQuery;
        this._discountQuery = discountQuery;
    }

    public async Task AddDiscounts(ShowMovie showMovie, ICollection<int> discounts)
    {
        var discountEntries = new List<Discount>(discounts.Count);

        foreach (var discount in discounts)
        {
            var d = await this._discountQuery.Handler(discount);

            if (d is null)
            {
                return;
            }

            discountEntries.Add(d);
        }

        showMovie.Discounts = discountEntries;
    }

    public async Task<int> Handler(CreateShowMovie request)
    {
        var showMovie = request.ShowMovie();

        var possible = await this._showMovieQuery.Handler(showMovie);

        //TODO: Check possible

        this._context.ShowMovies.Add(showMovie);
        await this._context.SaveChangesAsync();

        // await CreateAvailableSeats(showMovie);

        return showMovie.Id;
    }

    private async Task CreateAvailableSeats(ShowMovie showMovie)
    {
        for (var i = 1; i <= showMovie.Cinema.CantSeats; i++)
        {
            this._context.AvailableSeats.Add(new AvailableSeat { ShowMovieId = showMovie.Id, Number = i });
        }

        await this._context.SaveChangesAsync();
    }
}