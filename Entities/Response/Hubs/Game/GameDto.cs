namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class GameDto
{
    public string Code { get; set; }

    public GameDto(string code)
    {
        Code = code;
    }
}