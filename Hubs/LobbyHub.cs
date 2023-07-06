using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Entities.Databases.Lobby;
using PeepoGuessrApi.Entities.Response.Hubs;
using PeepoGuessrApi.Entities.Response.Hubs.Lobby;
using PeepoGuessrApi.Services.Interfaces.Lobby.Db;
using PeepoGuessrApi.Services.Interfaces.Maintenance.Db;

namespace PeepoGuessrApi.Hubs;

[Authorize]
public class LobbyHub : Hub
{
    private readonly ILogger<LobbyHub> _logger;
    private readonly Services.Interfaces.Account.Db.IUserService _accountUserService;
    private readonly Services.Interfaces.Account.Db.IGameStatusService _accountGameStatusService;
    private readonly Services.Interfaces.Account.Db.IGameService _accountGameService;
    private readonly Services.Interfaces.Game.Db.IGameService _gameGameService;
    private readonly ILobbyTypeService _lobbyTypeService;
    private readonly IUserService _userService;
    private readonly IWorkService _maintenanceWorkService;

    public LobbyHub(ILogger<LobbyHub> logger, Services.Interfaces.Account.Db.IUserService accountUserService,
        Services.Interfaces.Account.Db.IGameStatusService accountGameStatusService,
        Services.Interfaces.Account.Db.IGameService accountGameService,
        Services.Interfaces.Game.Db.IGameService gameGameService, ILobbyTypeService lobbyTypeService, 
        IUserService userService, IWorkService maintenanceWorkService)
    {
        _logger = logger;
        _accountUserService = accountUserService;
        _lobbyTypeService = lobbyTypeService;
        _accountGameStatusService = accountGameStatusService;
        _accountGameService = accountGameService;
        _gameGameService = gameGameService;
        _userService = userService;
        _maintenanceWorkService = maintenanceWorkService;
        _logger.LogInformation("Lobby Hub Startup [{Time}]", DateTimeOffset.UtcNow);
    }
    
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null || httpContext.User.Identity is { Name: null } or null ||
            !int.TryParse(httpContext.User.Identity.Name, out var userId))
        {
            _logger.LogInformation("Lobby Disconnected User [{Time}]: httpContext is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }

        var accountUser = await _accountUserService.Find(userId);
        if (accountUser == null)
        {
            _logger.LogInformation("Lobby Disconnected User [{Time}]: accountUser is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }

        if (accountUser.RoleId > 2)
        {
            var works = await _maintenanceWorkService.FindActive();
            if (works != null)
            {
                await Clients.Caller.SendAsync("MaintenanceExpire", new MaintenanceDto(works.Expire));
                _logger.LogInformation("Lobby Disconnected User [{Time}]: maintenance abort", DateTimeOffset.UtcNow);
                Context.Abort();
                return;
            }
        }

        var lobby = (string?)httpContext.GetRouteValue("lobby");
        var lobbyType = await _lobbyTypeService.Find(lobby!);
        if (lobbyType == null)
        {
            _logger.LogInformation("Lobby Disconnected User [{Time}]: lobbyType is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }
        
        var accountGameStatus = await _accountGameStatusService.Find("active");
        if (accountGameStatus == null)
        {
            _logger.LogInformation("Lobby Disconnected User [{Time}]: accountGameStatus is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }
        
        var game = await _accountGameService.FindByUserAndStatus(accountUser.Id, accountGameStatus);
        if (game != null)
        {
            var gameGame = await _gameGameService.Find(game.Id);
            if (gameGame != null)
            {
                await Clients.Caller.SendAsync("RedirectToGame", new GameDto(gameGame.Code, gameGame.RoundExpire));
                _logger.LogInformation("Lobby Disconnected User [{Time}]: user have active game", DateTimeOffset.UtcNow);
                Context.Abort();
                return;
            }
        }

        var user = await _userService.Find(accountUser.Id);
        if (user == null)
        {
            user = new User
            {
                UserId = accountUser.Id,
                ConnectionId = Context.ConnectionId,
                LobbyTypeId = lobbyType.Id,
                Name = accountUser.Name,
                ImageUrl = accountUser.ImageUrl,
                DivisionId = accountUser.DivisionId,
                Score = accountUser.Score
            };
            if (!await _userService.Create(user))
            {
                _logger.LogInformation("Lobby Disconnected User [{Time}]: user is null", DateTimeOffset.UtcNow);
                Context.Abort();
                return;
            }
        }
        else
        {
            _logger.LogInformation("Lobby Disconnected User [{Time}]: multiple user connections", DateTimeOffset.UtcNow);
            await Clients.Caller.SendAsync("Error");
            Context.Abort();
        }

        foreach (var lobbyUser in lobbyType.Users.Where(u => u.DivisionId == user.DivisionId).ToList())
        {
            await Clients.Caller.SendAsync("NewUser", new UserDto(lobbyUser.UserId, lobbyUser.Name, lobbyUser.ImageUrl,
                lobbyUser.DivisionId, lobbyUser.Score, lobbyUser.IsRandomAcceptable, lobbyUser.IsGameFounded));
        }
        await Clients.Group(user.LobbyType!.Name.ToLower() + "-" + user.DivisionId).SendAsync("NewUser", 
            new UserDto(user.UserId, user.Name, user.ImageUrl, user.DivisionId, user.Score, user.IsRandomAcceptable, 
                user.IsGameFounded));
        
        await Groups.AddToGroupAsync(Context.ConnectionId, user.LobbyType!.Name.ToLower() + "-" + user.DivisionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, "DelayHolder");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null || httpContext.User.Identity is { Name: null } or null ||
            !int.TryParse(httpContext.User.Identity.Name, out var userId))
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }
        var user = await _userService.Find(userId);
        if (user == null)
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }
        if (user.ConnectionId == Context.ConnectionId)
            await _userService.Remove(user.Id);
        await Clients.All.SendAsync("RemoveUser", new UserInviteDto(user.UserId));
        await base.OnDisconnectedAsync(exception);
    }
}