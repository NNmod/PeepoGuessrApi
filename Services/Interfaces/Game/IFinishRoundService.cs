using PeepoGuessrApi.Entities.Response.Hubs.Game;

namespace PeepoGuessrApi.Services.Interfaces.Game;

public interface IFinishRoundService
{
    public Task<(bool isGameFinish, RoundSummaryDto? summary)> CompleteNormal(Entities.Databases.Game.Game game, 
        bool countFromNearestUser = false);
}