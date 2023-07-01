using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Services.Interfaces.Game;
using PeepoGuessrApi.Services.Interfaces.Game.Db;

namespace PeepoGuessrApi.Services.Implementations.Game;

public class FinishGameService : IFinishGameService
{
    private readonly Interfaces.Account.Db.IGameService _accountGameService;
    private readonly Interfaces.Account.Db.IGameStatusService _accountGameStatusService;
    private readonly Interfaces.Account.Db.IRoundService _accountRoundService;
    private readonly Interfaces.Account.Db.IUserService _accountUserService;
    private readonly Interfaces.Account.Db.IDivisionService _accountDivisionService;
    private readonly Interfaces.Account.Db.ISummaryService _accountSummaryService;
    private readonly IGameService _gameService;
    
    public FinishGameService(Interfaces.Account.Db.IGameService accountGameService, 
        Interfaces.Account.Db.IGameStatusService accountGameStatusService,
        Interfaces.Account.Db.IRoundService accountRoundService, Interfaces.Account.Db.IUserService accountUserService,
        Interfaces.Account.Db.IDivisionService accountDivisionService, Interfaces.Account.Db.ISummaryService accountSummaryService,
        IGameService gameService)
    {
        _accountGameService = accountGameService;
        _accountGameStatusService = accountGameStatusService;
        _accountRoundService = accountRoundService;
        _accountUserService = accountUserService;
        _accountDivisionService = accountDivisionService;
        _accountSummaryService = accountSummaryService;
        _gameService = gameService;
    }
    
    public async Task<bool> Cancel(Entities.Databases.Game.Game game)
    {
        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
            return false;
        
        var accountGameStatus = await _accountGameStatusService.Find("canceled");
        if (accountGameStatus == null)
            return false;
        
        accountGame.GameStatusId = accountGameStatus.Id;
        if (!await _accountGameService.Update(accountGame))
            return false;
        
        return await _gameService.Remove(game.Id);
    }

    public async Task<GameSummaryDto?> CompleteNormal(Entities.Databases.Game.Game game, int summaryDelay, double ratio = 1, 
        int roundIgnoreCount = 0, bool isSingle = false)
    {
        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
            return null;
        
        var accountRound = await _accountRoundService.Find(accountGame, game.RoundCount);
        if (accountRound == null)
            return null;
        
        var userSummaries = new List<GameUserSummaryDto>();
        var rounds = game.RoundCount > 20 ? 20 : game.RoundCount;
        var winner = accountRound.RoundSummaries.MaxBy(u => u.Health);
        if (isSingle)
        {
            if (rounds <= 5)
                winner = null;
        }
        else
        {
            if (winner is { Health: 0 })
                winner = null;
        }

        foreach (var summary in accountGame.Summaries)
        {
            var accountUser = await _accountUserService.Find(summary.UserId);
            if (accountUser == null || summary.User == null)
                continue;
            
            var oldScore = accountUser.Score;
            var oldDivisionId = accountUser.DivisionId;
            var isWinner = false;
            
            if (winner?.UserId == summary.User.Id)
            {
                var score = (int)(rounds * 3 * ratio);
                if (roundIgnoreCount > rounds)
                    score = 0;
                else
                    accountUser.Wins++;
                accountUser.Score += score;
                isWinner = true;
            }
            else if (!isSingle)
            {
                var score = (int)((25 - rounds) * ratio);
                accountUser.Score -= score;
            }
            
            if (accountUser.Score < 0)
                accountUser.Score = 0;
            if (accountUser.Score > 1500)
                accountUser.Score = 1500;

            var division = await _accountDivisionService.FindByScore(accountUser.Score);
            accountUser.DivisionId = division?.Id ?? accountUser.DivisionId;
            
            var upgrade = (division?.MaxScore ?? accountUser.Division?.MaxScore ?? 0) - accountUser.Score;
            if (upgrade < 0)
                upgrade = 0;

            summary.DivisionId = accountUser.DivisionId;
            summary.Score = accountUser.Score;

            if (!await _accountUserService.Update(accountUser) || !await _accountSummaryService.Update(summary))
                continue;
            
            userSummaries.Add(new GameUserSummaryDto(summary.User.Id, accountUser.DivisionId, 
                accountUser.Score, oldScore, accountUser.Wins, upgrade, isWinner, oldDivisionId < accountUser.DivisionId, 
                oldDivisionId > accountUser.DivisionId));
        }
        
        var accountGameStatus = await _accountGameStatusService.Find("completed");
        if (accountGameStatus == null)
            return null;
        
        accountGame.GameStatusId = accountGameStatus.Id;
        if (!await _accountGameService.Update(accountGame))
            return null;
        
        if (await _gameService.Remove(game.Id))
            return new GameSummaryDto(DateTime.UtcNow.AddSeconds(summaryDelay), userSummaries);
        return null;
    }
}