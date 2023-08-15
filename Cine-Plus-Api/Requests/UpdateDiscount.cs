using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Requests;

public class UpdateDiscount : CreateDiscount
{
    public int Id { get; set; }

    public new Discount Discount() => new Discount
        { Id = this.Id, Name = this.Name, Description = this.Description, DiscountPercent = this.DiscountPercent };
}