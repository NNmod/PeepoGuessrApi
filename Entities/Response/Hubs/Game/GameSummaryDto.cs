namespace PeepoGuessrApi.Entities.Response.Hubs.Game;

public class GameSummaryDto
{
    public DateTime SummaryDelay { get; set; }
    public List<GameUserSummaryDto> Users { get; set; }

    public GameSummaryDto(DateTime summaryDelay, List<GameUserSummaryDto> users)
    {
        SummaryDelay = summaryDelay;
        Users = users;
    }
}