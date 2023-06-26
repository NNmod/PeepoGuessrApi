namespace PeepoGuessrApi.Entities.Response.Account.Game;

public class GameRoundDto
{
    public int Id { get; set; }
    public string MapName { get; set; }
    public int Count { get; set; }

    public GameRoundDto(int id, string mapName, int count)
    {
        Id = id;
        MapName = mapName;
        Count = count;
    }
}