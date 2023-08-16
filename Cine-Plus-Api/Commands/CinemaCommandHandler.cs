using Microsoft.EntityFrameworkCore;
using System.Net;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Commands;

public interface ICinemaCommandHandler
{
    Task<ApiResponse<int>> Handler(CreateCinema request);

    Task<ApiResponse> Handler(UpdateCinema request);

    Task<ApiResponse> Handler(int id);
}

public class CinemaCommandHandler : ICinemaCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly ICinemaQueryHandler _cinemaQuery;

    private readonly IShowMovieQueryHandler _showMovieQuery;

    public CinemaCommandHandler(CinePlusContext context, ICinemaQueryHandler cinemaQuery,
        IShowMovieQueryHandler showMovieQuery)
    {
        this._context = context;
        this._cinemaQuery = cinemaQuery;
        this._showMovieQuery = showMovieQuery;
    }

    public async Task<ApiResponse<int>> Handler(CreateCinema request)
    {
        var responseSameName = await CheckSameName(request.Name);
        if (!responseSameName.Ok) return responseSameName.ConvertApiResponse<int>();
        
        var cinema = request.Cinema();

        this._context.Cinemas.Add(cinema);
        await this._context.SaveChangesAsync();

        return new ApiResponse<int>(cinema.Id);
    }

    public async Task<ApiResponse> Handler(UpdateCinema request)
    {
        var responseCinema = await Find(request.Id);
        if (!responseCinema.Ok) return responseCinema.ConvertApiResponse();

        var responseSameName = await CheckSameName(request.Name, request.Id);
        if (!responseSameName.Ok) return responseSameName;

        var cinema = request.Cinema();

        this._context.Cinemas.Update(cinema);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    public async Task<ApiResponse> Handler(int id)
    {
        var responseCinema = await Find(id);
        if (!responseCinema.Ok) return responseCinema.ConvertApiResponse();

        var responseShowMovie = await FindInAvailableShowMovie(id);
        if (!responseShowMovie.Ok) return responseShowMovie;

        var cinema = responseCinema.Value!;

        this._context.Cinemas.Remove(cinema);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    private async Task<ApiResponse> CheckSameName(string name, int id = -1)
    {
        var cinemaEntry = await this._cinemaQuery.Handler(name);

        if (cinemaEntry is not null && cinemaEntry.Id != id)
            return new ApiResponse(HttpStatusCode.BadRequest, "There is already a cinema with the same name");

        return new ApiResponse();
    }

    private async Task<ApiResponse<Cinema>> Find(int id)
    {
        var cinema = await this._context.Cinemas.SingleOrDefaultAsync(cinema => cinema.Id == id);

        return cinema is null
            ? new ApiResponse<Cinema>(HttpStatusCode.NotFound, "Not found cinema")
            : new ApiResponse<Cinema>(cinema);
    }

    private async Task<ApiResponse> FindInAvailableShowMovie(int id)
    {
        var filter = await this._showMovieQuery.AvailableCinema(id);

        return filter.Count != 0
            ? new ApiResponse(HttpStatusCode.BadRequest, "There is a show movie available with this cinema")
            : new ApiResponse();
    }
}