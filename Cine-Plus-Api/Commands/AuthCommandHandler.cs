using System.Net;
using Cine_Plus_Api.Models;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IAuthCommandHandler
{
    Task<ApiResponse<int>> User(CreateUser request);

    Task<ApiResponse> User(UpdateUser request);
}

public class AuthCommandHandler : IAuthCommandHandler
{
    private readonly CinePlusContext _context;

    private readonly IAuthQueryHandler _authQuery;

    public AuthCommandHandler(CinePlusContext context, IAuthQueryHandler authQuery)
    {
        this._context = context;
        this._authQuery = authQuery;
    }

    public async Task<ApiResponse<int>> User(CreateUser request)
    {
        var responseSameEmail = await CheckSameEmail(request.Email);
        if (!responseSameEmail.Ok) return responseSameEmail.ConvertApiResponse<int>();

        var user = request.User();

        var responseValid = ValidUser(user);
        if (!responseValid.Ok) return responseValid.ConvertApiResponse<int>();

        this._context.Users.Add(user);
        await this._context.SaveChangesAsync();

        return new ApiResponse<int>(user.Id);
    }

    public async Task<ApiResponse> User(UpdateUser request)
    {
        var responseUser = await Find(request.Id);
        if (!responseUser.Ok) return responseUser.ConvertApiResponse();

        var user = responseUser.Value!;

        user.Name = request.Name;
        user.Password = request.Password;

        var responseValid = ValidUser(user);
        if (!responseValid.Ok) return responseValid;

        this._context.Users.Update(user);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    private async Task<ApiResponse<User>> Find(int id)
    {
        var user = await this._authQuery.User(id);

        return user is null
            ? new ApiResponse<User>(HttpStatusCode.NotFound, "Not found user")
            : new ApiResponse<User>(user);
    }

    private ApiResponse ValidUser(User user)
    {
        if (string.IsNullOrEmpty(user.Name)) return new ApiResponse(HttpStatusCode.BadRequest, "Name is required");
        if (string.IsNullOrEmpty(user.Email)) return new ApiResponse(HttpStatusCode.BadRequest, "Email is required");
        if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 5)
            return new ApiResponse(HttpStatusCode.BadRequest, "Password is required or invalid");

        return new ApiResponse();
    }

    private async Task<ApiResponse> CheckSameEmail(string email)
    {
        var response = await this._authQuery.User(email);

        return response is null
            ? new ApiResponse()
            : new ApiResponse(HttpStatusCode.BadRequest, "There is already a user with the same email");
    }
}