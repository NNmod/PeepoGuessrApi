using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Game;

public class GameType
{
    [Key]
    public int Id { get; set; }
    /// <summary>Unique name of the game mode</summary>
    public required string Name { get; set; }
    /// <summary>The round duration (in seconds) of the game mode</summary>
    public int RoundDuration { get; set; }
    /// <summary>The round promotion option of the game mode</summary>
    public bool IsPromotionEnable { get; set; }
    /// <summary>The round duration after promotion (in seconds) of the game mode</summary>
    public int RoundPromotionDuration { get; set; }
    
    /// <summary>List of games depends on this game mode</summary>
    public List<Game> Games { get; set; }

    public GameType()
    {
        Games = new List<Game>();
    }
}