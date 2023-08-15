using Cine_Plus_Api.Commands;

namespace Cine_Plus_Api.Services;

public interface IMovieCommandHandler
{
    Task<int> Handler(CreateMovie request);

    Task<int> Handler(UpdateMovie request);
}

public class MovieCommandHandler : IMovieCommandHandler
{
    private readonly CinePlusContext _context;

    public MovieCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<int> Handler(CreateMovie request)
    {
        var movie = request.Movie();

        this._context.Add(movie);
        await this._context.SaveChangesAsync();

        return movie.Id;
    }

    public async Task<int> Handler(UpdateMovie request)
    {
        var movie = request.Movie();

        this._context.Update(movie);
        await this._context.SaveChangesAsync();

        return movie.Id;
    }
}