using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Entities.Response.Hubs;
using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Services.Interfaces.Game.Db;
using PeepoGuessrApi.Services.Interfaces.Maintenance.Db;
using User = PeepoGuessrApi.Entities.Databases.Game.User;

namespace PeepoGuessrApi.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;
    private readonly Services.Interfaces.Account.Db.IUserService _accountUserService;
    private readonly Services.Interfaces.Account.Db.IGameService _accountGameService;
    private readonly Services.Interfaces.Account.Db.IRoundService _accountRoundService;
    private readonly IGameService _gameService;
    private readonly IUserService _userService;
    private readonly IWorkService _maintenanceWorkService;

    public GameHub(ILogger<GameHub> logger, Services.Interfaces.Account.Db.IUserService accountUserService, 
        Services.Interfaces.Account.Db.IGameService accountGameService,  
        Services.Interfaces.Account.Db.IRoundService accountRoundService,
        IGameService gameService, IUserService userService, IWorkService maintenanceWorkService)
    {
        _logger = logger;
        _accountUserService = accountUserService;
        _accountGameService = accountGameService;
        _accountRoundService = accountRoundService;
        _gameService = gameService;
        _userService = userService;
        _maintenanceWorkService = maintenanceWorkService;
        _logger.LogInformation("Game Hub Startup [{Time}]", DateTimeOffset.UtcNow);
    }
    
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null || httpContext.User.Identity is { Name: null } or null ||
            !int.TryParse(httpContext.User.Identity.Name, out var userId))
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: httpContext is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }
        
        var accountUser = await _accountUserService.Find(userId);
        if (accountUser == null)
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: accountUser is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }
        
        if (accountUser.RoleId > 2)
        {
            var works = await _maintenanceWorkService.FindActive();
            if (works != null)
            {
                await Clients.Caller.SendAsync("MaintenanceExpire", new MaintenanceDto(works.Expire));
                _logger.LogInformation("Game Disconnected User [{Time}]: maintenance abort", DateTimeOffset.UtcNow);
                Context.Abort();
                return;
            }
        }
        
        var user = await _userService.Find(accountUser.Id);
        if (user != null)
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: multiple user connections", DateTimeOffset.UtcNow);
            await Clients.Caller.SendAsync("Error");
            Context.Abort();
            return;
        }
        
        var gameCode = (string?)httpContext.GetRouteValue("game");
        var game = await _gameService.FindByCode(gameCode!);
        if (game == null)
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: game is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }

        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: accountGame is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }

        if (accountGame.Summaries.All(u => u.UserId != accountUser.Id) || accountGame.GameStatusId > 1)
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: user do not have access to the game", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
        
        user = new User
        {
            UserId = accountUser.Id,
            ConnectionId = Context.ConnectionId,
            GameId = game.Id,
            Name = accountUser.Name,
            ImageUrl = accountUser.ImageUrl,
            DivisionId = accountUser.DivisionId
        };
        
        Round? accountRound = null;
        if (game.RoundCount > 0)
        {
            accountRound = await _accountRoundService.Find(accountGame, game.RoundCount);
            if (accountRound == null)
            {
                _logger.LogInformation("Game Disconnected User [{Time}]: accountRound is null", DateTimeOffset.UtcNow);
                Context.Abort();
                return;
            }
        }
        
        if (!await _userService.Create(user))
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: user is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }
        
        if (accountGame.Summaries.Count == game.Users.Count + 1 && game.RoundCount == 0)
        {
            game.RoundExpire = DateTime.UtcNow.AddSeconds(5);
            await _gameService.Update(game);
        }
        
        user = await _userService.Find(user.UserId);
        if (user == null)
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: user^2 is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }

        foreach (var summary in accountGame.Summaries)
        {
            var roundUser = accountRound?.RoundSummaries.FirstOrDefault(u => u.UserId == summary.UserId);
            if (summary.User == null)
                continue;
            
            var isConnected = game.Users.Any(u => u.UserId == summary.User.Id) || user.UserId == summary.User.Id;

            await Clients.Caller.SendAsync("NewUser", new UserDto(user.UserId == summary.User?.Id ? 
                    user.ConnectionId ?? string.Empty : string.Empty, summary.User!.Id, isConnected, summary.User!.Name, 
                summary.User!.ImageUrl, summary.User!.DivisionId, game.RoundCount == 0 ? 5000 : roundUser?.Health ?? 5000, 
                game.RoundCount == 0 ? 0 : roundUser?.GuessAvailable ?? 0));
            
            if (game.RoundCount == 0 || roundUser == null)
                continue;
            if (roundUser.PosX == null || roundUser.PosY == null)
                continue;
            
            await Clients.Caller.SendAsync("UserGuess", new GuessDto(roundUser.UserId, roundUser.GuessAvailable, 
                roundUser.Distance));
        }
        
        await Clients.Group(game.Code).SendAsync("UserStatus", new UserStatusDto(user.UserId, true));
        
        if (user.Game?.RoundCount > 0)
            await Clients.Caller.SendAsync("NewRound",
                new RoundDto(user.Game.RoundCount, user.Game.Multiplier, user.Game.MapUrl,
                    user.Game.PosX, user.Game.PosY, game.RoundExpire, user.Game.RoundDelayExpire));
        
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
        {
            await _userService.Remove(user.Id);
            
            await Clients.Group(user.Game!.Code).SendAsync("UserStatus", new UserStatusDto(user.UserId, 
                false));
        }
        await base.OnDisconnectedAsync(exception);
    }
}