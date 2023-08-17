using System.Net;

namespace Cine_Plus_Api.Responses;

public class ApiResponse<T>
{
    public HttpStatusCode Status { get; private set; }

    public string? Message { get; private set; }

    public T? Value { get; set; }

    public bool Ok => HttpStatusCode.Accepted == Status;

    public ApiResponse(HttpStatusCode status, string message)
    {
        this.Message = message;
        this.Status = status;
    }

    public ApiResponse(T value)
    {
        this.Value = value;
        this.Status = HttpStatusCode.Accepted;
    }

    public ApiResponse<T1> ConvertApiResponse<T1>() => new(this.Status, this.Message!);
    public ApiResponse ConvertApiResponse() => new(this.Status, this.Message!);
}

public class ApiResponse
{
    public HttpStatusCode Status { get; private set; }

    public string? Message { get; private set; }

    public bool Ok => HttpStatusCode.Accepted == Status;

    public ApiResponse(HttpStatusCode status, string message)
    {
        this.Message = message;
        this.Status = status;
    }

    public ApiResponse()
    {
        this.Status = HttpStatusCode.Accepted;
    }

    public ApiResponse<T> ConvertApiResponse<T>() => new(this.Status, this.Message!);
}