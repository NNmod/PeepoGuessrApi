namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class GameSummaryDto
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public int DivisionId { get; set; }
    public int Score { get; set; }
    public int OldScore { get; set; }
    public bool IsWinner { get; set; }
    public bool IsDivisionPromoted { get; set; }
    public bool IsDivisionDemoted { get; set; }

    public GameSummaryDto(int userId, string name, string imageUrl, int divisionId, int score, int oldScore,
        bool isWinner, bool isDivisionPromoted, bool isDivisionDemoted)
    {
        UserId = userId;
        Name = name;
        ImageUrl = imageUrl;
        DivisionId = divisionId;
        Score = score;
        OldScore = oldScore;
        IsWinner = isWinner;
        IsDivisionPromoted = isDivisionPromoted;
        IsDivisionDemoted = isDivisionDemoted;
    }
}