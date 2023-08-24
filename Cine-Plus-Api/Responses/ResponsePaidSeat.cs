namespace Cine_Plus_Api.Responses;

public class ResponsePaidSeat
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public string Description { get; set; } = null!;
}