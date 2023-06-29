namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class GameUserSummaryDto
{
    public int UserId { get; set; }
    public int DivisionId { get; set; }
    public int Score { get; set; }
    public int OldScore { get; set; }
    public int Wins { get; set; }
    public int Upgrade { get; set; }
    public bool IsWinner { get; set; }
    public bool IsDivisionPromoted { get; set; }
    public bool IsDivisionDemoted { get; set; }

    public GameUserSummaryDto(int userId, int divisionId, int score, int oldScore, int wins, int upgrade, bool isWinner, 
        bool isDivisionPromoted, bool isDivisionDemoted)
    {
        UserId = userId;
        DivisionId = divisionId;
        Score = score;
        OldScore = oldScore;
        Wins = wins;
        Upgrade = upgrade;
        IsWinner = isWinner;
        IsDivisionPromoted = isDivisionPromoted;
        IsDivisionDemoted = isDivisionDemoted;
    }
}