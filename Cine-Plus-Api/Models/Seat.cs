using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public enum SeatState
{
    Available,
    Reserved,
    Bought
}

public class Seat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public double Price { get; set; }

    [Required] public int Number { get; set; }

    [Required] public SeatState State { get; set; }

    public DateTime RowVersion { get; set; }

    [ForeignKey("ShowMovie")] public int ShowMovieId { get; set; }

    public ShowMovie ShowMovie { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = null!;

    public ICollection<Discount> Discounts { get; set; } = null!;
}