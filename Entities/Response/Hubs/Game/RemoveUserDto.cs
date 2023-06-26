namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class RemoveUserDto
{
    public string ConnectionId { get; set; }
    public int UserId { get; set; }

    public RemoveUserDto(string connectionId, int userId)
    {
        ConnectionId = connectionId;
        UserId = userId;
    }
}