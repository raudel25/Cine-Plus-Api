using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public double Price { get; set; }

    [Required] public int PricePoints { get; set; }

    [Required] public int AddPoints { get; set; }

    [Required] public bool Paid { get; set; }

    public ICollection<Seat> Seats { get; set; } = null!;

    public ICollection<Pay> Pays { get; set; } = null!;
    
    public ICollection<ReturnPay> ReturnPays { get; set; } = null!;
}