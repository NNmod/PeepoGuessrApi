using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Entities;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Hubs;
using PeepoGuessrApi.Services.Interfaces.Game;
using PeepoGuessrApi.Services.Interfaces.Game.Db;
using Game = PeepoGuessrApi.Entities.Databases.Game.Game;
using GameType = PeepoGuessrApi.Entities.Databases.Game.GameType;

namespace PeepoGuessrApi.HostedServices;

public class GameHostedService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger<GameHostedService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly IGameService _gameService;
    private readonly IGameTypeService _gameTypeService;
    private readonly IMapService _mapService;
    private readonly IUserService _userService;
    private readonly Services.Interfaces.Account.Db.IMapService _accountMapService;
    private readonly Services.Interfaces.Account.Db.IRoundService _accountRoundService;
    private readonly Services.Interfaces.Account.Db.IRoundSummaryService _accountRoundSummaryService;
    private readonly Services.Interfaces.Account.Db.IGameService _accountGameService;
    private readonly Services.Interfaces.Account.Db.IUserService _accountUserService;
    private readonly Services.Interfaces.Account.Db.IDivisionService _accountDivisionService;
    private readonly Services.Interfaces.Account.Db.IGameStatusService _accountGameStatusService;
    private readonly Services.Interfaces.Account.Db.ISummaryService _accountSummaryService;

    public GameHostedService(ILogger<GameHostedService> logger, IConfiguration configuration, 
        IHubContext<GameHub> gameHubContext, IGameService gameService, IGameTypeService gameTypeService, 
        IMapService mapService, Services.Interfaces.Account.Db.IMapService accountMapService,
        IUserService userService, Services.Interfaces.Account.Db.IRoundService accountRoundService, 
        Services.Interfaces.Account.Db.IRoundSummaryService accountRoundSummaryService,
        Services.Interfaces.Account.Db.IGameService accountGameService, 
        Services.Interfaces.Account.Db.IUserService accountUserService,
        Services.Interfaces.Account.Db.IDivisionService accountDivisionService,
        Services.Interfaces.Account.Db.IGameStatusService accountGameStatusService,
        Services.Interfaces.Account.Db.ISummaryService accountSummaryService)
    {
        _semaphore = new SemaphoreSlim(1);
        _logger = logger;
        _configuration = configuration;
        _gameHubContext = gameHubContext;
        _gameService = gameService;
        _gameTypeService = gameTypeService;
        _mapService = mapService;
        _userService = userService;
        _accountMapService = accountMapService;
        _accountRoundService = accountRoundService;
        _accountRoundSummaryService = accountRoundSummaryService;
        _accountGameService = accountGameService;
        _accountUserService = accountUserService;
        _accountDivisionService = accountDivisionService;
        _accountGameStatusService = accountGameStatusService;
        _accountSummaryService = accountSummaryService;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Game Hosted Service Startup [{Time}]", DateTime.UtcNow);
        _timer = new Timer(async _ =>
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                await DoWork();
            }
            finally
            {
                _semaphore.Release();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        
        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        await CheckGameTypeGames("singleplayer");
        //await CheckGameTypeGames("multiplayer");
    }

    private async Task CheckGameTypeGames(string gameTypeName)
    {
        _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop started", DateTime.UtcNow, gameTypeName);
        var games = await _gameTypeService.FindInclude(gameTypeName);
        if (games == null)
            return;
        var dateTime = DateTime.UtcNow;
        foreach (var game in games.Games)
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update", 
                DateTime.UtcNow, gameTypeName, game.Code);
            if (game.RoundExpire > dateTime)
                continue;
            if (game.RoundCount == 0)
            {
                await StartGameRound(games, game, 15);
                _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update: new round started", 
                    DateTime.UtcNow, gameTypeName, game.Code);
                continue;
            }

            if (game.Users.Count == 0)
            {
                await CancelGame(game);
                _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update: game canceled", 
                    DateTime.UtcNow, gameTypeName, game.Code);
                continue;
            }

            bool isLastRound;
            switch (gameTypeName)
            {
                case "singleplayer":
                    isLastRound = await CompleteClassicGameRound(game);
                    _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update: game round completed", 
                        DateTime.UtcNow, gameTypeName, game.Code);
                    break;
                default:
                    isLastRound = await CompleteClassicGameRound(game, true);
                    _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update: game round completed", 
                        DateTime.UtcNow, gameTypeName, game.Code);
                    break;
            }
            if (isLastRound)
                switch (gameTypeName)
                {
                    case "singleplayer":
                        await CompleteClassicGame(game, 15, 0.33, 5);
                        _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update: game completed", 
                            DateTime.UtcNow, gameTypeName, game.Code);
                        break;
                    case "multiplayer":// or "randomEvents":
                        await CompleteClassicGame(game, 15);
                        _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update: game completed", 
                            DateTime.UtcNow, gameTypeName, game.Code);
                        break;
                }
            else
            {
                await StartGameRound(games, game, 15);
                _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update: new round started^2", 
                    DateTime.UtcNow, gameTypeName, game.Code);
            }
        }
        _logger.LogInformation("Game Hosted Service [{Time}]: Game {GameTypeName} update loop finished", DateTime.UtcNow, gameTypeName);
    }

    private async Task CancelGame(Game game)
    {
        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: accountGame^4 is null", DateTime.UtcNow);
            return;
        }
        var accountGameStatus = await _accountGameStatusService.Find("canceled");
        if (accountGameStatus == null)
            return;
        accountGame.GameStatusId = accountGameStatus.Id;
        if (!await _accountGameService.Update(accountGame))
            return;
        await _gameService.Remove(game.Id);
    }

    private async Task CompleteClassicGame(Game game, int summaryDelay, double ratio = 1, int roundIgnoreCount = 0)
    {
        var rounds = game.RoundCount > 20 ? 20 : game.RoundCount;
        var winner = game.Users.MaxBy(u => u.Health);
        if (winner == null)
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: winner is null", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return;
        }

        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: accountGame is null", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return;
        }
        
        var userSummaries = new List<GameUserSummaryDto>();

        foreach (var user in game.Users)
        {
            var removeUserResult = await _userService.Remove(user.Id);
            if (!removeUserResult)
                continue;
            var accountUser = await _accountUserService.Find(user.UserId);
            var accountSummary = accountGame.Summaries.FirstOrDefault(s => s.UserId == user.UserId);
            if (accountUser == null || accountSummary == null)
                continue;
            var oldScore = accountUser.Score;
            var oldDivisionId = accountUser.DivisionId;
            var isWinner = false;
            if (winner.UserId == user.UserId)
            {
                var score = (int)((rounds * 3) * ratio);
                if (roundIgnoreCount > rounds)
                    score = 0;
                else
                    accountUser.Wins++;
                accountUser.Score += score;
                isWinner = true;
            }
            else
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

            accountSummary.DivisionId = accountUser.DivisionId;
            accountSummary.Score = accountUser.Score;

            if (!await _accountUserService.Update(accountUser) || !await _accountSummaryService.Update(accountSummary))
                continue;
            
            userSummaries.Add(new GameUserSummaryDto(user.UserId, user.Name, user.ImageUrl, accountUser.DivisionId, 
                accountUser.Score, oldScore, isWinner, oldDivisionId < accountUser.DivisionId, 
                oldDivisionId > accountUser.DivisionId));
        }
        
        var accountGameStatus = await _accountGameStatusService.Find("completed");
        if (accountGameStatus == null)
            return;
        accountGame.GameStatusId = accountGameStatus.Id;
        if (!await _accountGameService.Update(accountGame))
            return;
        await _gameService.Remove(game.Id);

        await _gameHubContext.Clients.Group(game.Code).SendAsync("GameSummary", new GameSummaryDto(
            DateTime.UtcNow.AddSeconds(summaryDelay), userSummaries));
    }

    private async Task<bool> CompleteClassicGameRound(Game game, bool countFromNearestUser = false)
    {
        await _gameHubContext.Clients.Group(game.Code).SendAsync("CompleteRound");
        
        var isLastRound = false;
        var nearestUser = game.Users.MinBy(u => u.Distance);
        var subtractor = countFromNearestUser ? nearestUser?.Distance ?? 0 : 0;
        
        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: accountGame^2 is null", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return false;
        }
        var accountRound = await _accountRoundService.Find(accountGame, game.RoundCount);
        if (accountRound == null)
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: accountRound is null", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return false;
        }

        var userSummaries = new List<RoundUserSummaryDto>();
        
        foreach (var user in game.Users)
        {
            var userRoundSummary = accountRound.RoundSummaries.FirstOrDefault(rs => rs.UserId == user.UserId);
            if (userRoundSummary == null)
            {
                _logger.LogInformation("Game Hosted Service [{Time}]: userRoundSummary is null", DateTime.UtcNow);
                await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
                return false;
            }

            var damage = (int)((user.Distance - subtractor) * game.Multiplier);
            user.Health -= damage;
            if (user.Health <= 0)
            {
                user.Health = 0;
                isLastRound = true;
            }

            userRoundSummary.PosX = user.PosX;
            userRoundSummary.PosY = user.PosY;
            userRoundSummary.Distance = user.Distance;
            userRoundSummary.Damage = damage;
            userRoundSummary.Health = user.Health;

            if (!await _accountRoundSummaryService.Update(userRoundSummary) || !await _userService.Update(user))
            {
                _logger.LogInformation("Game Hosted Service [{Time}]: accountRoundSummary or user cannot be updated", DateTime.UtcNow);
                await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
                return false;
            }
            
            userSummaries.Add(new RoundUserSummaryDto(user.ConnectionId, user.UserId, user.Name, user.ImageUrl, 
                user.DivisionId, userRoundSummary.Health, userRoundSummary.Damage, userRoundSummary.Distance, 
                userRoundSummary.PosX, userRoundSummary.PosY));
        }
        
        await _gameHubContext.Clients.Group(game.Code).SendAsync("RoundSummary", new RoundSummaryDto(isLastRound,
            accountRound.Count, accountRound.Map!.Url, accountRound.PosX, accountRound.PosY, userSummaries));
        
        return isLastRound;
    }

    private async Task StartGameRound(GameType gameType, Game game, int roundDelay, int customRoundDuration = 0)
    {
        game.RoundCount++;
        var maps = await _accountMapService.FindClassic();
        var rnd = new Random();
        var mapId = rnd.Next(1, maps.Count + 1);
        var map = maps.FirstOrDefault(m => m.Id == mapId);
        var position = await _mapService.RandomPosition(_configuration["Cdn:Maps"]!, map!.Url) ?? new Point
        {
            X = rnd.Next(1000),
            Y = rnd.Next(1000)
        };

        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: accountGame^3 is null", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return;
        } 
        
        var round = new Round
        {
            GameId = game.GameId,
            MapId = map.Id,
            Count = game.RoundCount,
            PosX = position.X,
            PosY = position.Y
        };
        if (!await _accountRoundService.Create(round))
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: accountRound cannot be created", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return;
        }

        round = await _accountRoundService.Find(accountGame, game.RoundCount);
        if (round == null)
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: accountRound is null", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return;
        } 

        foreach (var user in game.Users)
        {
            var roundSummary = new RoundSummary
            {
                RoundId = round.Id,
                UserId = user.UserId,
                Health = user.Health,
                Distance = 5000,
                PosX = null,
                PosY = null
            };
            user.PosX = null;
            user.PosY = null;
            user.Distance = 5000;
            user.GuessAvailable = 1;
            if (await _accountRoundSummaryService.Create(roundSummary) && await _userService.Update(user))
            {
                await _gameHubContext.Clients.Group(game.Code).SendAsync("UserGuess", new GuessDto(user.UserId, 
                    user.GuessAvailable, user.Distance));
                continue;
            }
            _logger.LogInformation("Game Hosted Service [{Time}]: accountRoundSummary or user cannot be created/updated", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return;
        }

        var dateTime = DateTime.UtcNow;
        game.MapUrl = map.Url;
        game.Multiplier = game.RoundCount > 5 ? 2 + 0.5 * (game.RoundCount - 5) : 0.75 + 0.25 * game.RoundCount;
        game.PosX = position.X;
        game.PosY = position.Y;
        game.IsRoundPromoted = false;
        game.RoundExpire = dateTime.AddSeconds(customRoundDuration == 0 ? gameType.RoundDuration :
            customRoundDuration).AddSeconds(roundDelay);
        game.RoundDelayExpire = dateTime.AddSeconds(roundDelay);
        
        if (!await _gameService.Update(game))
        {
            _logger.LogInformation("Game Hosted Service [{Time}]: game cannot be updated", DateTime.UtcNow);
            await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
            return;
        }
        
        await _gameHubContext.Clients.Group(game.Code).SendAsync("NewRound",
            new RoundDto(game.RoundCount, game.Multiplier, game.MapUrl,
                position.X, position.Y, game.RoundExpire, game.RoundDelayExpire));
    }
    
    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Game Hosted Service Shutdown [{Time}]", DateTime.UtcNow);
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}