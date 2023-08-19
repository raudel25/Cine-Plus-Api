using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public class AvailableSeat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public double Price { get; set; }

    [Required] public int Number { get; set; }

    [Required] public bool Available { get; set; }

    [ForeignKey("ShowMovie")] public int ShowMovieId { get; set; }

    public ShowMovie ShowMovie { get; set; } = null!;
}