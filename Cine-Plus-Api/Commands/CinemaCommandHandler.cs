using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;
using Cine_Plus_Api.Responses;
using System.Net;

namespace Cine_Plus_Api.Commands;

public interface ICinemaCommandHandler
{
    Task<ApiResponse<int>> Handler(CreateCinema request);

    Task<ApiResponse> Handler(UpdateCinema request);

    Task Handler(int id);
}

public class CinemaCommandHandler : ICinemaCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly ICinemaQueryHandler _cinemaQuery;

    public CinemaCommandHandler(CinePlusContext context, ICinemaQueryHandler cinemaQuery)
    {
        this._context = context;
        this._cinemaQuery = cinemaQuery;
    }

    public async Task<ApiResponse<int>> Handler(CreateCinema request)
    {
        var cinema = request.Cinema();

        var cinemaEntry = await this._cinemaQuery.Handler(request.Name);

        if (cinemaEntry is not null)
            return new ApiResponse<int>(HttpStatusCode.BadRequest, "There is already a cinema with the same name");

        this._context.Cinemas.Add(cinema);
        await this._context.SaveChangesAsync();

        return new ApiResponse<int>(cinema.Id);
    }

    public async Task<ApiResponse> Handler(UpdateCinema request)
    {
        var cinema = request.Cinema();

        var cinemaEntry = await this._cinemaQuery.Handler(request.Name);

        if (cinemaEntry is not null && cinemaEntry.Id != cinema.Id)
            return new ApiResponse(HttpStatusCode.BadRequest, "There is already a cinema with the same name");

        this._context.Cinemas.Update(cinema);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    public async Task Handler(int id)
    {
        var cinema = await this._context.Cinemas.SingleOrDefaultAsync(cinema => cinema.Id == id);

        if (cinema is null) return;

        this._context.Cinemas.Remove(cinema);
        await this._context.SaveChangesAsync();
    }
}