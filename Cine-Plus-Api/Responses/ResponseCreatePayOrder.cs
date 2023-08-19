namespace Cine_Plus_Api.Responses;

public class ResponseCreatePayOrder
{
    public bool Ok => this.Token is not null;

    public string? Token { get; set; }

    public ICollection<ResponseSeatOrder> Seats { get; set; } = null!;
}

public class ResponseSeatOrder
{
    public int Id { get; set; }

    public bool Ok => this.Message == "Ok";

    public string Message { get; set; } = null!;
}