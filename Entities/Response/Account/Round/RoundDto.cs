namespace PeepoGuessrApi.Entities.Response.Account.Round;

public class RoundDto
{
    public int Id { get; set; }
    public string MapName { get; set; }
    public int Count { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    public List<RoundUserDto> Users { get; set; }

    public RoundDto(int id, string mapName, int count, double posX, double posY, List<RoundUserDto> users)
    {
        Id = id;
        MapName = mapName;
        Count = count;
        PosX = posX;
        PosY = posY;
        Users = users;
    }
}