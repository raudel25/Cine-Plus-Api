using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public class ShowMovie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public long Date { get; set; }

    [ForeignKey("Movie")] public int MovieId { get; set; }

    public Movie Movie { get; set; } = null!;

    [ForeignKey("Cinema")] public int CinemaId { get; set; }

    public Cinema Cinema { get; set; } = null!;

    public ICollection<Discount> Discounts { get; set; } = null!;
}