namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class RoundDto
{
    public int RoundCount { get; set; }
    public double Multiplier { get; set; }
    public string MapUrl { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    public DateTime RoundExpire { get; set; }
    public DateTime RoundDelay { get; set; }

    public RoundDto(int roundCount, double multiplier, string mapUrl, double posX, double posY,
        DateTime roundExpire, DateTime roundDelay)
    {
        RoundCount = roundCount;
        Multiplier = multiplier;
        MapUrl = mapUrl;
        PosX = posX;
        PosY = posY;
        RoundExpire = roundExpire;
        RoundDelay = roundDelay;
    }
}