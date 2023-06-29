using PeepoGuessrApi.Entities.Response.Hubs.Game;

namespace PeepoGuessrApi.Services.Interfaces.Game;

public interface IFinishGameService
{
    public Task<bool> Cancel(Entities.Databases.Game.Game game);
    public Task<GameSummaryDto?> CompleteNormal(Entities.Databases.Game.Game game, int summaryDelay, double ratio = 1, 
        int roundIgnoreCount = 0);
}