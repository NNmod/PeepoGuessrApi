using PeepoGuessrApi.Entities;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Services.Interfaces.Game;
using PeepoGuessrApi.Services.Interfaces.Game.Db;
using GameType = PeepoGuessrApi.Entities.Databases.Game.GameType;

namespace PeepoGuessrApi.Services.Implementations.Game;

public class StartRoundService : IStartRoundService
{
    private readonly Interfaces.Account.Db.IMapService _accountMapService;
    private readonly Interfaces.Account.Db.IGameService _accountGameService;
    private readonly Interfaces.Account.Db.IRoundService _accountRoundService;
    private readonly Interfaces.Account.Db.IRoundSummaryService _accountRoundSummaryService;
    private readonly IGameService _gameService;
    private readonly IMapService _mapService;
    
    public StartRoundService(Interfaces.Account.Db.IMapService accountMapService, 
        Interfaces.Account.Db.IGameService accountGameService, Interfaces.Account.Db.IRoundService accountRoundService,
        Interfaces.Account.Db.IRoundSummaryService accountRoundSummaryService, IGameService gameService, IMapService mapService)
    {
        _accountMapService = accountMapService;
        _accountGameService = accountGameService;
        _accountRoundService = accountRoundService;
        _accountRoundSummaryService = accountRoundSummaryService;
        _gameService = gameService;
        _mapService = mapService;
    }
    
    public async Task<(bool isSuccess, RoundDto? round, List<GuessDto> guesses)> StartNormal(string mapCdnUrl, GameType gameType, 
        Entities.Databases.Game.Game game, int roundDelay, int customRoundDuration = 0)
    {
        var maps = await _accountMapService.FindClassic();
        var rnd = new Random();
        var mapId = rnd.Next(1, maps.Count + 1);
        var map = maps.FirstOrDefault(m => m.Id == mapId);
        var position = await _mapService.RandomPosition(mapCdnUrl, map!.Url) ?? new Point
        {
            X = rnd.Next(1000),
            Y = rnd.Next(1000)
        };

        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
            return (false, null, new List<GuessDto>());
        
        var round = new Round
        {
            GameId = game.GameId,
            MapId = map.Id,
            Count = game.RoundCount + 1,
            PosX = position.X,
            PosY = position.Y
        };
        if (!await _accountRoundService.Create(round))
            return (false, null, new List<GuessDto>());

        var guesses = new List<GuessDto>();
        round = await _accountRoundService.Find(accountGame, game.RoundCount + 1);
        if (round == null)
            return (false, null, new List<GuessDto>());

        if (game.RoundCount > 0)
        {
            var lastRound = await _accountRoundService.Find(accountGame, game.RoundCount);
            if (lastRound == null)
                return (false, null, new List<GuessDto>());
            
            foreach (var user in lastRound.RoundSummaries)
            {
                if (round.RoundSummaries.Any(u => u.UserId == user.UserId))
                {
                    var savedUser = round.RoundSummaries.FirstOrDefault(u => u.UserId == user.UserId);
                    if (savedUser != null)
                        guesses.Add(new GuessDto(savedUser.UserId, savedUser.GuessAvailable, savedUser.Distance));
                    continue;
                }
                
                var roundSummary = new RoundSummary
                {
                    RoundId = round.Id,
                    UserId = user.UserId,
                    Health = user.Health,
                    GuessAvailable = 1,
                    Distance = 5000,
                    PosX = null,
                    PosY = null
                };
                if (!await _accountRoundSummaryService.Create(roundSummary))
                    return (false, null, new List<GuessDto>());
                
                guesses.Add(new GuessDto(roundSummary.UserId, roundSummary.GuessAvailable, roundSummary.Distance));
            }
        }
        else
        {
            foreach (var user in accountGame.Summaries)
            {
                if (round.RoundSummaries.Any(u => u.UserId == user.UserId))
                {
                    var savedUser = round.RoundSummaries.FirstOrDefault(u => u.UserId == user.UserId);
                    if (savedUser != null)
                        guesses.Add(new GuessDto(savedUser.UserId, savedUser.GuessAvailable, savedUser.Distance));
                    continue;
                }
                
                var roundSummary = new RoundSummary
                {
                    RoundId = round.Id,
                    UserId = user.UserId,
                    Health = 5000,
                    GuessAvailable = 1,
                    Distance = 5000,
                    PosX = null,
                    PosY = null
                };
                if (!await _accountRoundSummaryService.Create(roundSummary))
                    return (false, null, new List<GuessDto>());
                
                guesses.Add(new GuessDto(roundSummary.UserId, roundSummary.GuessAvailable, roundSummary.Distance));
            }
        }

        var dateTime = DateTime.UtcNow;
        game.RoundCount++;
        game.MapUrl = map.Url;
        game.Multiplier = game.RoundCount > 5 ? 2 + 0.5 * (game.RoundCount - 5) : 0.75 + 0.25 * game.RoundCount;
        game.PosX = position.X;
        game.PosY = position.Y;
        game.IsRoundPromoted = false;
        game.RoundExpire = dateTime.AddSeconds(customRoundDuration == 0 ? gameType.RoundDuration :
            customRoundDuration).AddSeconds(roundDelay);
        game.RoundDelayExpire = dateTime.AddSeconds(roundDelay);
        
        return await _gameService.Update(game) ? (true, new RoundDto(game.RoundCount, game.Multiplier, game.MapUrl,
            position.X, position.Y, game.RoundExpire, game.RoundDelayExpire), guesses) : (false, null, new List<GuessDto>());
    }
}