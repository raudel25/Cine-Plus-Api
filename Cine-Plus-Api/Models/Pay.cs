using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public class Pay
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public long Date { get; set; }

    [ForeignKey("Order")] public int OrderId { get; set; }

    public Order Order { get; set; } = null!;
}

public class CreditCard : Pay
{
    public long Card { get; set; }
}

public class PointsUser : Pay
{
    [ForeignKey("User")] public int UserId { get; set; }

    public User User { get; set; } = null!;
}

public class Ticket : Pay
{
    [ForeignKey("Employ")] public int EmployId { get; set; }

    public Employ Employ { get; set; } = null!;
}