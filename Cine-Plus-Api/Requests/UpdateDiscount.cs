using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class UpdateDiscount : CreateDiscount
{
    public int Id { get; set; }

    public override Discount Discount()
    {
        var discount = base.Discount();
        discount.Id = this.Id;

        return discount;
    }
}