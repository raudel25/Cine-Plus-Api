using System.Net;
using Cine_Plus_Api.Commands;
using Cine_Plus_Api.Helpers;
using Cine_Plus_Api.Queries;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Responses;

namespace Cine_Plus_Api.Services;

public class Auth
{
    private readonly IAuthCommandHandler _authCommand;

    private readonly IAuthQueryHandler _authQuery;

    private readonly Token _token;

    public Auth(IAuthCommandHandler authCommand, Token token, IAuthQueryHandler authQuery)
    {
        this._authCommand = authCommand;
        this._token = token;
        this._authQuery = authQuery;
    }

    public async Task<ApiResponse<AuthResponse>> CreateUser(CreateUser request)
    {
        request.Password = Password.EncryptPassword(request.Password);

        var response = await this._authCommand.User(request);
        if (!response.Ok) return response.ConvertApiResponse<AuthResponse>();

        var token = this._token.Jwt(response.Value, AccountType.User);

        return new ApiResponse<AuthResponse>(new AuthResponse(response.Value, token, AccountType.User));
    }

    public async Task<ApiResponse<AuthResponse>> LoginUser(LoginUser request)
    {
        var user = await this._authQuery.User(request.Email);

        if (user is null || Password.CheckPassword(request.Password, user.Password))
            return new ApiResponse<AuthResponse>(HttpStatusCode.BadRequest, "Incorrect email or password");

        var token = this._token.Jwt(user.Id, AccountType.User);

        return new ApiResponse<AuthResponse>(new AuthResponse(user.Id, token, AccountType.User));
    }
}