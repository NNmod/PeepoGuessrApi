using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Entities.Databases.Lobby;
using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Hubs;
using PeepoGuessrApi.Services.Interfaces.Account.Db;
using PeepoGuessrApi.Services.Interfaces.Lobby;
using PeepoGuessrApi.Services.Interfaces.Lobby.Db;
using IUserService = PeepoGuessrApi.Services.Interfaces.Lobby.Db.IUserService;

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
    private readonly Services.Interfaces.Game.Db.IGameService _gameGameService;
    private readonly IStartGameService _startGameService;

    public LobbyHostedService(ILogger<LobbyHostedService> logger, IConfiguration configuration, IHubContext<LobbyHub> lobbyHubContext,
        ILobbyTypeService lobbyTypeService, IUserService userService, IDivisionService accountDivisionService,
        Services.Interfaces.Game.Db.IGameService gameGameService, IStartGameService startGameService)
    {
        _semaphore = new SemaphoreSlim(1);
        _logger = logger;
        _configuration = configuration;
        _lobbyHubContext = lobbyHubContext;
        _lobbyTypeService = lobbyTypeService;
        _userService = userService;
        _accountDivisionService = accountDivisionService;
        _gameGameService = gameGameService;
        _startGameService = startGameService;
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
        
        var singleplayer = await _lobbyTypeService.FindInclude("singleplayer");
        if (singleplayer != null)
        {
            foreach (var user in singleplayer.Users)
            {
                var code = Guid.NewGuid().ToString();
                if (await _startGameService.StartSingleGame(user, "singleplayer", code))
                    await _lobbyHubContext.Clients.Client(user.ConnectionId).SendAsync("GameFound", new GameDto(code));
                else
                    await _lobbyHubContext.Clients.Client(user.ConnectionId).SendAsync("MatchmakingTrouble");
            }
        }
        
        var multiplayer = await _lobbyTypeService.FindInclude("multiplayer");
        if (multiplayer != null)
        {
            foreach (var division in await _accountDivisionService.FindAll())
            {
                /*var users = multiplayer.Users.Where(d => d.DivisionId == division.Id && d.IsRandomAcceptable)
                    .ToList();
                for (var i = 0; i < users.Count -1; i += 2)
                {
                    var user1 = users[i];
                    var user2 = users[i + 1];
                    var code = Guid.NewGuid().ToString();
                    if (await _startGameService.StartMultiplayerGame(user1, user2, "multiplayer", code))
                    {
                        await _lobbyHubContext.Clients.Client(user1.ConnectionId).SendAsync("GameFound", new GameDto(code));
                        await _lobbyHubContext.Clients.Client(user2.ConnectionId).SendAsync("GameFound", new GameDto(code));
                        continue;
                    }
                    await _lobbyHubContext.Clients.Client(user1.ConnectionId).SendAsync("MatchmakingTrouble");
                    await _lobbyHubContext.Clients.Client(user2.ConnectionId).SendAsync("MatchmakingTrouble");
                }*/
                
                var inviteUsers = multiplayer.Users.Where(d => d.DivisionId == division.Id)
                    .ToList();
                var ignoreInviteUsers = new List<User>();
                foreach (var user in inviteUsers)
                {
                    if (ignoreInviteUsers.Any(u => u.Id == user.Id))
                        continue;

                    var invitedUser = inviteUsers.FirstOrDefault(
                        u => u.Id != user.Id && u.Invites.Any(ui => ui.InvitedUserId == user.UserId) 
                                             && user.Invites.Any(ui => ui.InvitedUserId == u.UserId) || 
                             u.Id != user.Id && u.IsRandomAcceptable && user.Invites.Any(ui => ui.InvitedUserId == u.UserId) ||
                             u.Id != user.Id && u.IsRandomAcceptable && user.IsRandomAcceptable);
                    if (invitedUser == null)
                        continue;
                    
                    var code = Guid.NewGuid().ToString();
                    if (await _startGameService.StartMultiplayerGame(user, invitedUser, "multiplayer", code))
                    {
                        ignoreInviteUsers.Add(invitedUser);
                        await _lobbyHubContext.Clients.Client(user.ConnectionId).SendAsync("GameFound", new GameDto(code));
                        await _lobbyHubContext.Clients.Client(invitedUser.ConnectionId).SendAsync("GameFound", new GameDto(code));
                        break;
                    }
                    await _lobbyHubContext.Clients.Client(user.ConnectionId).SendAsync("MatchmakingTrouble");
                    await _lobbyHubContext.Clients.Client(invitedUser.ConnectionId).SendAsync("MatchmakingTrouble");
                }
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