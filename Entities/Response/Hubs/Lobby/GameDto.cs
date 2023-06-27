namespace PeepoGuessrApi.Entities.Response.Hubs.Lobby;

public class GameDto
{
    public string GameCode { get; set; }
    public DateTime GameExpire { get; set; }

    public GameDto(string gameCode, DateTime gameExpire)
    {
        GameCode = gameCode;
        GameExpire = gameExpire;
    }
}