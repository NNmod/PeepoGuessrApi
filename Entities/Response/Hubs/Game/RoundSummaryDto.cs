namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class RoundSummaryDto
{
    public bool IsGameCompleted { get; set; }
    public int RoundCount { get; set; }
    public string MapUrl { get; set; }
    public double PosX { get; set; }
    public double PosY { get; set; }
    
    public List<RoundUserSummaryDto> Users { get; set; }

    public RoundSummaryDto(bool isGameCompleted, int roundCount, string mapUrl, double posX, double posY, 
        List<RoundUserSummaryDto> users)
    {
        IsGameCompleted = isGameCompleted;
        RoundCount = roundCount;
        MapUrl = mapUrl;
        PosX = posX;
        PosY = posY;
        Users = users;
    }
}