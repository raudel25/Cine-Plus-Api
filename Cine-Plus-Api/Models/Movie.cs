using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] public bool CubanCine { get; set; }

    [Required] public int Rating { get; set; }

    [Required] public long Duration { get; set; }

    [ForeignKey("Director")] public int DirectorId { get; set; }

    public Director Director { get; set; } = null!;

    [ForeignKey("Genre")] public int GenreId { get; set; }

    public Genre Genre { get; set; } = null!;

    public ICollection<Actor> Actors { get; set; } = null!;
}