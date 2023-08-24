namespace Cine_Plus_Api.Requests;

public class PayCreditCard
{
    public string Token { get; set; } = null!;
    public long CreditCard { get; set; }

    public double Amount { get; set; }
}