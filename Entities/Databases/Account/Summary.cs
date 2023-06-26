using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class Summary
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Game")]
    public int GameId { get; set; }
    public virtual Game? Game { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int DivisionId { get; set; }
    public int Score { get; set; }
}