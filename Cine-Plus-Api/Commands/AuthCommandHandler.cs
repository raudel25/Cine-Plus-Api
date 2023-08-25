using System.Net;
using Cine_Plus_Api.Helpers;
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

    Task User(User user, int points);

    Task<CreateEmployResponse> Employ();

    Task<CreateManagerResponse> Manager();

    Task<ApiResponse> Employ(int id);

    Task<ApiResponse> Manager(int id);
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
        var responseSameEmail = await CheckSameEmailIdentityCard(request.Email, request.IdentityCard);
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
        var responseUser = await FindUser(request.Id);
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

    public async Task User(User user, int points)
    {
        user.Points = points;
        this._context.Users.Update(user);
        await this._context.SaveChangesAsync();
    }


    public async Task<CreateEmployResponse> Employ()
    {
        var number = await this._authQuery.MaxEmploy() + 1;
        var password = Password.RandomPassword();
        var user = $"employ{number}";

        var employ = new Employ { User = user, Password = password };

        this._context.Employs.Add(employ);
        await this._context.SaveChangesAsync();

        return new CreateEmployResponse { User = user, Password = password };
    }

    public async Task<CreateManagerResponse> Manager()
    {
        var number = await this._authQuery.MaxManager() + 1;
        var password = Password.RandomPassword();
        var user = $"manager{number}";

        var manager = new Manager { User = user, Password = password };

        this._context.Managers.Add(manager);
        await this._context.SaveChangesAsync();

        return new CreateManagerResponse { User = user, Password = password };
    }

    public async Task<ApiResponse> Employ(int id)
    {
        var responseEmploy = await FindEmploy(id);
        if (!responseEmploy.Ok) return responseEmploy.ConvertApiResponse();

        var employ = responseEmploy.Value!;

        this._context.Employs.Remove(employ);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    public async Task<ApiResponse> Manager(int id)
    {
        var responseManager = await FindManager(id);
        if (!responseManager.Ok) return responseManager.ConvertApiResponse();

        var manager = responseManager.Value!;

        this._context.Managers.Remove(manager);
        await this._context.SaveChangesAsync();

        return new ApiResponse();
    }

    private async Task<ApiResponse<User>> FindUser(int id)
    {
        var user = await this._authQuery.UserId(id);

        return user is null
            ? new ApiResponse<User>(HttpStatusCode.NotFound, "Not found user")
            : new ApiResponse<User>(user);
    }

    private async Task<ApiResponse<Employ>> FindEmploy(int id)
    {
        var employ = await this._authQuery.Employ(id);

        return employ is null
            ? new ApiResponse<Employ>(HttpStatusCode.NotFound, "Not found user")
            : new ApiResponse<Employ>(employ);
    }

    private async Task<ApiResponse<Manager>> FindManager(int id)
    {
        var manager = await this._authQuery.Manager(id);

        return manager is null
            ? new ApiResponse<Manager>(HttpStatusCode.NotFound, "Not found user")
            : new ApiResponse<Manager>(manager);
    }

    private ApiResponse ValidUser(User user)
    {
        if (string.IsNullOrEmpty(user.Name)) return new ApiResponse(HttpStatusCode.BadRequest, "Name is required");
        if (string.IsNullOrEmpty(user.Email)) return new ApiResponse(HttpStatusCode.BadRequest, "Email is required");
        if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 5)
            return new ApiResponse(HttpStatusCode.BadRequest, "Password is required or invalid");

        return new ApiResponse();
    }

    private async Task<ApiResponse> CheckSameEmailIdentityCard(string email, long ic)
    {
        var response1 = await this._authQuery.UserEmail(email);
        var response2 = await this._authQuery.UserIdentityCard(ic);

        return response1 is null && response2 is null
            ? new ApiResponse()
            : new ApiResponse(HttpStatusCode.BadRequest, "There is already a user with the same email or name");
    }
}