using Cine_Plus_Api.Models;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IPayCommandHandler
{
    Task CreditCard(int id, PayCreditCard request);

    Task Ticket(int id, int employId);
}

public class PayCommandHandler : IPayCommandHandler
{
    private readonly CinePlusContext _context;

    public PayCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task CreditCard(int id, PayCreditCard request)
    {
        var now = DateTime.UtcNow;
        var creditCard = new CreditCard
            { Card = request.CreditCard, Date = ((DateTimeOffset)now).ToUnixTimeSeconds(), OrderId = id };

        this._context.CreditCards.Add(creditCard);
        await this._context.SaveChangesAsync();
    }

    public async Task Ticket(int id, int employId)
    {
        var now = DateTime.UtcNow;
        var ticket = new Ticket
            { EmployId = employId, Date = ((DateTimeOffset)now).ToUnixTimeSeconds(), OrderId = id };

        this._context.Tickets.Add(ticket);
        await this._context.SaveChangesAsync();
    }
}