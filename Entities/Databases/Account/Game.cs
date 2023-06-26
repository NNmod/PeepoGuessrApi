using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class Game
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("GameType")]
    public int GameTypeId { get; set; }
    public virtual GameType? GameType { get; set; }
    [ForeignKey("GameStatus")]
    public int GameStatusId { get; set; }
    public virtual GameStatus? GameStatus { get; set; }
    public required string Code { get; set; }
    public DateTime DateTime { get; set; }

    public List<Round> Rounds { get; set; }
    public List<Summary> Summaries { get; set; }
    

    public Game()
    {
        DateTime = DateTime.UtcNow;
        Rounds = new List<Round>();
        Summaries = new List<Summary>();
    }
}