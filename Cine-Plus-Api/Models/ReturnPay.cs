using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cine_Plus_Api.Models;

public class ReturnPay
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public long Date { get; set; }

    [ForeignKey("Seat")] public int SeatId { get; set; }

    public Seat Seat { get; set; } = null!;

    [ForeignKey("Order")] public int OrderId { get; set; }

    public Order Order { get; set; } = null!;
}

public class ReturnCreditCard : ReturnPay
{
    public double Amount { get; set; }
}

public class ReturnPointsUser : ReturnPay
{
    public int Points { get; set; }
}

