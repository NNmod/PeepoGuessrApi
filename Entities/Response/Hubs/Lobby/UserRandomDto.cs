namespace PeepoGuessrApi.Entities.Response.Hubs.Lobby;

public class UserRandomDto
{
    public int UserId { get; set; }
    public bool IsRandomAcceptable { get; set; }

    public UserRandomDto(int userId, bool isRandomAcceptable)
    {
        UserId = userId;
        IsRandomAcceptable = isRandomAcceptable;
    }
}