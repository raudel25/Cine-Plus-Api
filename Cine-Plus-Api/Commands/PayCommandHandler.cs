using Cine_Plus_Api.Models;
using Cine_Plus_Api.Requests;
using Cine_Plus_Api.Services;

namespace Cine_Plus_Api.Commands;

public interface IPayCommandHandler
{
    Task CreditCard(int id,PayCreditCard request);
}

public class PayCommandHandler
{
    private readonly CinePlusContext _context;

    public PayCommandHandler(CinePlusContext context)
    {
        this._context = context;
    }
    
    public async Task CreditCard(int id,PayCreditCard request)
    {
        var creditCard = new CreditCard { Card = request.CreditCard, OrderId = id };

        this._context.CreditCards.Add(creditCard);
        await this._context.SaveChangesAsync();
    }
}