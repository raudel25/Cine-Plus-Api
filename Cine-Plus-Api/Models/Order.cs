using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public double Price { get; set; }

    [Required] public bool Paid { get; set; }

    public ICollection<Seat> Seats { get; set; } = null!;
    
    //TODO
    public ICollection<Order> Orders { get; set; } = null!;
}