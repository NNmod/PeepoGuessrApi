using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Lobby;

public class UserInvite
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    [ForeignKey("InvitedUser")]
    public int InvitedUserId { get; set; }
    public virtual User? InvitedUser { get; set; }
}