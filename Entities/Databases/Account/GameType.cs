using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class GameType
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public List<Game> Games { get; set; }

    public GameType()
    {
        Games = new List<Game>();
    }
}