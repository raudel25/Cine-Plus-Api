using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] public string Email { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;

    [Required] public int Points { get; set; }

    public ICollection<PointsUser> PointsUsers { get; set; } = null!;

    public ICollection<TicketPointsUser> TicketPointsUsers { get; set; } = null!;
}