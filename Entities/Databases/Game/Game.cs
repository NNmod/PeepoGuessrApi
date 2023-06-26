using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Game;

public class Game
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("GameType")]
    public int GameTypeId { get; set; }
    public virtual GameType? GameType { get; set; }
    public int GameId { get; set; }
    public required string Code { get; set; }
    public int RoundCount { get; set; }
    public required string MapUrl { get; set; }
    public double Multiplier { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    public bool IsRoundPromoted { get; set; }
    public DateTime RoundExpire { get; set; }
    public DateTime RoundDelayExpire { get; set; }
    
    public List<User> Users { get; set; }

    public Game()
    {
        RoundExpire = DateTime.UtcNow.AddMinutes(1);
        RoundDelayExpire = DateTime.UtcNow.AddSeconds(5);
        Users = new List<User>();
    }
}