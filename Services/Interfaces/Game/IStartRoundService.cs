using PeepoGuessrApi.Entities.Databases.Game;
using PeepoGuessrApi.Entities.Response.Hubs.Game;

namespace PeepoGuessrApi.Services.Interfaces.Game;

public interface IStartRoundService
{
    public Task<(bool isSuccess, RoundDto? round, List<GuessDto> guesses)> StartNormal(string mapCdnUrl, GameType gameType,
        Entities.Databases.Game.Game game, int roundDelay, int customRoundDuration = 0);
}