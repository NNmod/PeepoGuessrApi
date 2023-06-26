using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class Round
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Game")]
    public int GameId { get; set; }
    public virtual Game? Game { get; set; }
    [ForeignKey("Map")]
    public int MapId { get; set; }
    public virtual Map? Map { get; set; }
    public int Count { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    
    public List<RoundSummary> RoundSummaries { get; set; }

    public Round()
    {
        RoundSummaries = new List<RoundSummary>();
    }
}