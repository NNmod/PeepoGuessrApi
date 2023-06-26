using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Lobby;

public class LobbyType
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public List<User> Users { get; set; }

    public LobbyType()
    {
        Users = new List<User>();
    }
}