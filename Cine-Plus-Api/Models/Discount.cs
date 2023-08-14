using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public class Discount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] public string Description { get; set; } = null!;

    [Required] private double DiscountPercent { get; set; }

    public ICollection<ShowMovie> ShowMovies { get; set; } = null!;
}