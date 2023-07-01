using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Hubs;
using PeepoGuessrApi.Services.Interfaces.Game;
using PeepoGuessrApi.Services.Interfaces.Game.Db;

namespace PeepoGuessrApi.HostedServices;

public class GameHostedService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger<GameHostedService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly IGameTypeService _gameTypeService;
    private readonly IFinishGameService _finishGameService;
    private readonly IFinishRoundService _finishRoundService;
    private readonly IStartRoundService _startRoundService;

    public GameHostedService(ILogger<GameHostedService> logger, IConfiguration configuration, IHubContext<GameHub> gameHubContext,
        IGameTypeService gameTypeService, IFinishGameService finishGameService, IFinishRoundService finishRoundService,
        IStartRoundService startRoundService)
    {
        _semaphore = new SemaphoreSlim(1);
        _logger = logger;
        _configuration = configuration;
        _gameHubContext = gameHubContext;
        _gameTypeService = gameTypeService;
        _finishGameService = finishGameService;
        _finishRoundService = finishRoundService;
        _startRoundService = startRoundService;
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
        await CheckGameTypeGames("multiplayer");
    }

    private async Task CheckGameTypeGames(string gameTypeName)
    {
        var dateTime = DateTime.UtcNow;
        var games = await _gameTypeService.FindInclude(gameTypeName);
        if (games == null)
            return;
        
        foreach (var game in games.Games)
        {
            _logger.LogDebug("Game Hosted Service [{Time}]: Game {GameTypeName} update loop: {GameCode} update", 
                DateTime.UtcNow, gameTypeName, game.Code);
            if (game.RoundExpire > dateTime)
                continue;
            if (game.RoundCount == 0)
            {
                var startRound = await _startRoundService.StartNormal(_configuration["Cdn:Maps"]!, games, game, 15);
                if (startRound.isSuccess)
                {
                    foreach (var guess in startRound.guesses)
                        await _gameHubContext.Clients.Group(game.Code).SendAsync("UserGuess", guess);
                    if (startRound.round != null)
                        await _gameHubContext.Clients.Group(game.Code).SendAsync("NewRound", startRound.round);
                }
                else
                    await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
                continue;
            }

            if (game.Users.Count == 0)
            {
                if (!await _finishGameService.Cancel(game))
                    await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
                continue;
            }

            var completeRound = gameTypeName == "singleplayer" ? await _finishRoundService.CompleteNormal(game)
                : await _finishRoundService.CompleteNormal(game, true);
            if (completeRound.summary != null)
                await _gameHubContext.Clients.Group(game.Code).SendAsync("RoundSummary", completeRound.summary);
            else
            {
                await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
                continue;
            }

            if (!completeRound.isGameFinish)
            {
                var startRound = await _startRoundService.StartNormal(_configuration["Cdn:Maps"]!, games, game, 15);
                if (startRound.isSuccess)
                {
                    foreach (var guess in startRound.guesses)
                        await _gameHubContext.Clients.Group(game.Code).SendAsync("UserGuess", guess);
                    if (startRound.round != null)
                        await _gameHubContext.Clients.Group(game.Code).SendAsync("NewRound", startRound.round);
                }
                else
                    await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
                continue;
            }

            var completeGame = gameTypeName == "singleplayer" ? await _finishGameService.CompleteNormal(game, 15, 0.33, 5, true)
                : await _finishGameService.CompleteNormal(game, 15);

            if (completeGame != null)
                await _gameHubContext.Clients.Group(game.Code).SendAsync("GameSummary", completeGame);
            else
                await _gameHubContext.Clients.Group(game.Code).SendAsync("Error");
        }
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