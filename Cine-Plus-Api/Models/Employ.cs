using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Models;

[Index(nameof(User), IsUnique = true)]
public class Employ
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public string User { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;

    public ICollection<Ticket> Tickets { get; set; } = null!;
    
    public ICollection<TicketPointsUser> TicketPointsUsers { get; set; } = null!;
}