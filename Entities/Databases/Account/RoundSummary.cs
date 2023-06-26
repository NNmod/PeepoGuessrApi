using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class RoundSummary
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Round")]
    public int RoundId { get; set; }
    public virtual Round? Round { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public double Distance { get; set; }
    public double? PosX { get; set; }
    public double? PosY { get; set; }
}