using Cine_Plus_Api.Models;

namespace Cine_Plus_Api.Helpers;

public static class Calculate
{
    public static double CalculatePrice(Seat seat)
    {
        var d = seat.Discounts.Select(discount => discount.DiscountPercent).Sum();
        return (100 - d) * seat.Price / 100;
    }

    public static int CalculatePricePoints(Seat seat)
    {
        var d = seat.Discounts.Select(discount => discount.DiscountPercent).Sum();
        return (int)((100 - d) * seat.PricePoints / 100);
    }
}