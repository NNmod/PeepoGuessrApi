namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class UserStatusDto
{
    public int UserId { get; set; }
    public bool IsConnected { get; set; }

    public UserStatusDto(int userId, bool isConnected)
    {
        UserId = userId;
        IsConnected = isConnected;
    }
}