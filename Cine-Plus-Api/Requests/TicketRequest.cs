namespace Cine_Plus_Api.Requests;

public class TicketRequest
{
    public GeneratePayOrder Order { get; set; } = null!;
}

public class TicketRequestUser : TicketRequest
{
    public int Id { get; set; }
}