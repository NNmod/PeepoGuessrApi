using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Services.Interfaces.Account.Db;
using PeepoGuessrApi.Services.Interfaces.Game;

namespace PeepoGuessrApi.Services.Implementations.Game;

public class FinishRoundService : IFinishRoundService
{
    private readonly IGameService _accountGameService;
    private readonly IRoundService _accountRoundService;
    private readonly IRoundSummaryService _accountRoundSummaryService;
    
    public FinishRoundService(IGameService accountGameService, IRoundService accountRoundService,
        IRoundSummaryService accountRoundSummaryService)
    {
        _accountGameService = accountGameService;
        _accountRoundService = accountRoundService;
        _accountRoundSummaryService = accountRoundSummaryService;
    }

    public async Task<(bool isGameFinish, RoundSummaryDto? summary)> CompleteNormal(Entities.Databases.Game.Game game, 
        bool countFromNearestUser = false)
    {
        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
            return (false, null);
        
        var accountRound = await _accountRoundService.Find(accountGame, game.RoundCount);
        if (accountRound == null)
            return (false, null);

        var isLastRound = false;
        var nearestUser = accountRound.RoundSummaries.MinBy(u => u.Distance);
        var subtractor = countFromNearestUser ? nearestUser?.Distance ?? 0 : 0;
        var userSummaries = new List<RoundUserSummaryDto>();
        
        foreach (var user in accountRound.RoundSummaries)
        {
            var damage = (int)((user.Distance - subtractor) * game.Multiplier);
            user.Health -= damage;
            user.Damage = damage;
            if (user.Health <= 0)
            {
                user.Health = 0;
                isLastRound = true;
            }

            if (!await _accountRoundSummaryService.Update(user))
                continue;
            
            userSummaries.Add(new RoundUserSummaryDto(user.User!.Id, user.User.Name, user.User.ImageUrl, 
                user.User.DivisionId, user.Health, user.Damage, user.Distance, user.PosX, user.PosY));
        }

        return (isLastRound, new RoundSummaryDto(isLastRound, accountRound.Count, accountRound.Map!.Url,
            accountRound.PosX, accountRound.PosY, userSummaries));
    }
}