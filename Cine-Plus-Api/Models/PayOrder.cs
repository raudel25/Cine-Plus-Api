using System.ComponentModel.DataAnnotations;

namespace Cine_Plus_Api.Models;

public class PayOrder
{
    [Key] public int Id { get; set; }

    [Required] public double Price { get; set; }

    [Required] public bool Paid { get; set; }

    public ICollection<PaidSeat> PaidSeats { get; set; } = null!;
}