using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Lobby;

public class User
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string ConnectionId { get; set; }
    [ForeignKey("LobbyType")]
    public int LobbyTypeId { get; set; }
    public virtual LobbyType? LobbyType { get; set; }
    public required string Name { get; set; }
    public required string ImageUrl { get; set; }
    public int DivisionId { get; set; }
    public int Score { get; set; }
    public bool IsRandomAcceptable { get; set; }
    public bool IsGameFounded { get; set; }
    
    public List<UserInvite> UserInvites { get; set;}

    public User()
    {
        UserInvites = new List<UserInvite>();
    }
}