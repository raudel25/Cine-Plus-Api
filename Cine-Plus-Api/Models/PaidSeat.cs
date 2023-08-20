using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public class PaidSeat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public double Price { get; set; }

    [Required] public int Number { get; set; }

    [ForeignKey("PayOrders")] public int PayOrderId { get; set; }

    public PayOrder PayOrder { get; set; } = null!;
    
    [ForeignKey("ShowMovie")] public int ShowMovieId { get; set; }

    public ShowMovie ShowMovie { get; set; } = null!;

    public ICollection<Discount> Discounts { get; set; } = null!;
}