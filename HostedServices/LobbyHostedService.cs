using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Entities.Databases.Lobby;
using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Hubs;
using PeepoGuessrApi.Services.Interfaces.Account.Db;
using PeepoGuessrApi.Services.Interfaces.LobbyDb;
using IUserService = PeepoGuessrApi.Services.Interfaces.LobbyDb.IUserService;
using UserDto = PeepoGuessrApi.Entities.Response.Hubs.Lobby.UserDto;

namespace PeepoGuessrApi.HostedServices;

public class LobbyHostedService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger<LobbyHostedService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<LobbyHub> _lobbyHubContext;
    private readonly ILobbyTypeService _lobbyTypeService;
    private readonly IUserService _userService;
    private readonly IDivisionService _accountDivisionService;
    private readonly IGameService _accountGameService;
    private readonly IGameStatusService _accountGameStatusService;
    private readonly ISummaryService _accountSummaryService;
    private readonly IGameTypeService _accountGameTypeService;
    private readonly Services.Interfaces.Game.Db.IGameService _gameGameService;
    private readonly Services.Interfaces.Game.Db.IGameTypeService _gameGameTypeService;

    public LobbyHostedService(ILogger<LobbyHostedService> logger, IConfiguration configuration, IHubContext<LobbyHub> lobbyHubContext,
        IDivisionService accountDivisionService, ILobbyTypeService lobbyTypeService, IUserService userService, 
        IGameService accountGameService, IGameStatusService accountGameStatusService, ISummaryService accountSummaryService, 
        IGameTypeService accountGameTypeService, Services.Interfaces.Game.Db.IGameService gameGameService,
        Services.Interfaces.Game.Db.IGameTypeService gameGameTypeService)
    {
        _semaphore = new SemaphoreSlim(1);
        _logger = logger;
        _configuration = configuration;
        _lobbyHubContext = lobbyHubContext;
        _accountDivisionService = accountDivisionService;
        _lobbyTypeService = lobbyTypeService;
        _userService = userService;
        _accountGameService = accountGameService;
        _accountGameStatusService = accountGameStatusService;
        _accountSummaryService = accountSummaryService;
        _accountGameTypeService = accountGameTypeService;
        _gameGameService = gameGameService;
        _gameGameTypeService = gameGameTypeService;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Online Hosted Service Startup [{Time}]", DateTime.UtcNow);
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
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));

        return Task.CompletedTask;
    }

    private async Task DoWork()
    {
        var gamesCount = await _gameGameService.Count();
        if (int.TryParse(_configuration["Game:Limit"], out var limit) && gamesCount >= limit)
        {
            await _lobbyHubContext.Clients.Group("DelayHolder").SendAsync("Delayed");
            var users = await _userService.FindList();
            foreach (var user in users)
                await _lobbyHubContext.Groups.RemoveFromGroupAsync(user.ConnectionId, "DelayHolder");
            return;
        }
        
        await CheckGame("singleplayer");
        //await CheckGame("multiplayer");
    }

    private async Task CheckGame(string gameTypeName)
    {
        var lobby = await _lobbyTypeService.FindInclude(gameTypeName);
        if (lobby == null)
            return;
        var lobbyAccountGameType = await _accountGameTypeService.Find(gameTypeName);
        if (lobbyAccountGameType == null)
            return;
        var lobbyGameGameType = await _gameGameTypeService.Find(gameTypeName);
        if (lobbyGameGameType == null)
            return;
        switch (gameTypeName)
        {
            case "singleplayer":
                await CreateSingleGames(lobby, lobbyAccountGameType, lobbyGameGameType);
                break;
            case "multiplayer":// or "randomEvents":
                await CreateClassicGames(lobby, lobbyAccountGameType, lobbyGameGameType);
                break;
        }
    }

    private async Task CreateSingleGames(LobbyType lobby, GameType accountGameType, Entities.Databases.Game.GameType gameGameType)
    {
        foreach (var user in lobby.Users)
        {
            var firstUser = user.IsGameFounded ? null : user;
            if (firstUser == null)
                continue;

            var accountGameStatus = await _accountGameStatusService.Find("active");
            if (accountGameStatus == null)
                continue;
                
            var accountGame = new Game
            {
                GameTypeId = accountGameType.Id,
                GameStatusId = accountGameStatus.Id,
                Code = Guid.NewGuid().ToString(),
                DateTime = DateTime.UtcNow
            };
            
            if (!await _accountGameService.Create(accountGame))
                continue;
            accountGame = await _accountGameService.FindByCode(accountGame.Code);
            if (accountGame == null)
                continue;
                
            var game = new Entities.Databases.Game.Game
            {
                GameTypeId = gameGameType.Id,
                GameId = accountGame.Id,
                Code = accountGame.Code,
                MapUrl = "",
                RoundCount = 0,
                Multiplier = 0,
                RoundExpire = DateTime.UtcNow.AddSeconds(5)
            };

            if (!await _gameGameService.Create(game))
            {
                accountGame.GameStatusId = 2;
                await _accountGameService.Update(accountGame);
                continue;
            }

            await _accountSummaryService.Create(new Summary
            {
                GameId = game.GameId,
                UserId = firstUser.UserId,
                Score = firstUser.Score
            });

            firstUser.IsGameFounded = true;
            await _userService.Update(firstUser);

            await _lobbyHubContext.Clients.Client(firstUser.ConnectionId).SendAsync("GameFound", new GameDto(game.Code));
        }
    }
    
    private async Task CreateClassicGames(LobbyType lobby, GameType accountGameType, Entities.Databases.Game.GameType gameGameType)
    {
        foreach (var division in await _accountDivisionService.FindAll())
        {
            while (lobby.Users.Count(u => u.DivisionId == division.Id && !u.IsGameFounded) > 1)
            {
                var firstUser = lobby.Users.FirstOrDefault();
                var lastUser = lobby.Users.LastOrDefault();
                if (firstUser == null || lastUser == null || firstUser == lastUser)
                    continue;

                await _lobbyHubContext.Clients.Client(firstUser.ConnectionId).SendAsync("EnemyFound",
                    new UserDto(lastUser.UserId, lastUser.Name, lastUser.ImageUrl, lastUser.DivisionId, lastUser.Score));
                await _lobbyHubContext.Clients.Client(lastUser.ConnectionId).SendAsync("EnemyFound",
                    new UserDto(firstUser.UserId, firstUser.Name, firstUser.ImageUrl, firstUser.DivisionId, firstUser.Score));

                var accountGameStatus = await _accountGameStatusService.Find("active");
                if (accountGameStatus == null)
                {
                    await _lobbyHubContext.Clients.Client(firstUser.ConnectionId).SendAsync("EnemyRevoke");
                    await _lobbyHubContext.Clients.Client(lastUser.ConnectionId).SendAsync("EnemyRevoke");
                    continue;
                }
                
                var accountGame = new Game
                {
                    GameTypeId = accountGameType.Id,
                    GameStatusId = accountGameStatus.Id,
                    Code = Guid.NewGuid().ToString(),
                    DateTime = DateTime.UtcNow
                };

                if (!await _accountGameService.Create(accountGame))
                {
                    await _lobbyHubContext.Clients.Client(firstUser.ConnectionId).SendAsync("EnemyRevoke");
                    await _lobbyHubContext.Clients.Client(lastUser.ConnectionId).SendAsync("EnemyRevoke");
                    continue;
                }
                accountGame = await _accountGameService.FindByCode(accountGame.Code);
                if (accountGame == null)
                {
                    await _lobbyHubContext.Clients.Client(firstUser.ConnectionId).SendAsync("EnemyRevoke");
                    await _lobbyHubContext.Clients.Client(lastUser.ConnectionId).SendAsync("EnemyRevoke");
                    continue;
                }
                
                var game = new Entities.Databases.Game.Game
                {
                    GameTypeId = gameGameType.Id,
                    GameId = accountGame.Id,
                    Code = accountGame.Code,
                    MapUrl = "",
                    RoundCount = 0,
                    Multiplier = 0,
                    RoundExpire = DateTime.UtcNow.AddSeconds(15)
                };

                if (await _gameGameService.Create(game))
                {
                    accountGame.GameStatusId = 2;
                    await _accountGameService.Update(accountGame);
                    await _lobbyHubContext.Clients.Client(firstUser.ConnectionId).SendAsync("EnemyRevoke");
                    await _lobbyHubContext.Clients.Client(lastUser.ConnectionId).SendAsync("EnemyRevoke");
                    continue;
                }

                await _accountSummaryService.Create(new Summary
                {
                    GameId = game.GameId,
                    UserId = firstUser.UserId,
                    Score = firstUser.Score
                });
                await _accountSummaryService.Create(new Summary
                {
                    GameId = game.GameId,
                    UserId = lastUser.UserId,
                    Score = lastUser.Score
                });

                firstUser.IsGameFounded = true;
                lastUser.IsGameFounded = true;
                await _userService.Update(firstUser);
                await _userService.Update(lastUser);
                
                await _lobbyHubContext.Clients.Client(firstUser.ConnectionId).SendAsync("GameFound", new GameDto(game.Code));
                await _lobbyHubContext.Clients.Client(lastUser.ConnectionId).SendAsync("GameFound", new GameDto(game.Code));
            }
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Online Hosted Service Shutdown [{Time}]", DateTime.UtcNow);
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}