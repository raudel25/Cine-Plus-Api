using System.ComponentModel.DataAnnotations;

namespace Cine_Plus_Api.Models;

public class Order
{
    [Key] public int Id { get; set; }

    [Required] public double Price { get; set; }

    [Required] public bool Paid { get; set; }
    
    public ICollection<Seat> Seats { get; set; } = null!;
}