namespace PeepoGuessrApi.Entities.Response.Hubs.Lobby;

public class UserInviteDto
{
    public int UserId { get; set; }

    public UserInviteDto(int userId)
    {
        UserId = userId;
    }
}