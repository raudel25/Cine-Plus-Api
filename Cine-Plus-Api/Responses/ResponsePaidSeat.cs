namespace Cine_Plus_Api.Responses;

public class ResponsePaidSeat
{
    public int Id { get; private set; }

    public string? Token { get; private set; }

    public string Description { get; private set; }

    public ResponsePaidSeat(int id, string? token, string description)
    {
        this.Id = id;
        this.Description = description;
        this.Token = token;
    }
}