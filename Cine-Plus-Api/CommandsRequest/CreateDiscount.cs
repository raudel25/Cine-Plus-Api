using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.CommandsRequest;

public class CreateDiscount
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double DiscountPercent { get; set; }

    public Discount Discount() => new Discount
        { Name = this.Name, Description = this.Description, DiscountPercent = this.DiscountPercent };
}