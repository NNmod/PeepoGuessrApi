using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Entities.Databases.Game;
using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Services.Interfaces.Game.Db;

namespace PeepoGuessrApi.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;
    private readonly Services.Interfaces.Account.Db.IUserService _accountUserService;
    private readonly Services.Interfaces.Account.Db.IGameService _accountGameService;
    private readonly IGameService _gameService;
    private readonly IUserService _userService;
    
    public GameHub(ILogger<GameHub> logger, Services.Interfaces.Account.Db.IUserService accountUserService, 
        Services.Interfaces.Account.Db.IGameService accountGameService, IGameService gameService, IUserService userService)
    {
        _logger = logger;
        _accountUserService = accountUserService;
        _accountGameService = accountGameService;
        _gameService = gameService;
        _userService = userService;
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
        
        var user = await _userService.Find(accountUser.Id);
        if (user == null)
        {
            user = new User
            {
                UserId = accountUser.Id,
                ConnectionId = Context.ConnectionId,
                GameId = game.Id,
                Name = accountUser.Name,
                ImageUrl = accountUser.ImageUrl,
                DivisionId = accountUser.DivisionId,
                GuessAvailable = 1,
                Health = 5000,
                Distance = 5000
            };
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
            await Clients.Group(game.Code).SendAsync("NewUser", new UserDto(
                user.ConnectionId!, user.UserId, user.Name, user.ImageUrl, user.DivisionId, user.Health, user.GuessAvailable));
        }
        else
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: multiple user connections", DateTimeOffset.UtcNow);
            await Clients.Caller.SendAsync("Error");
            Context.Abort();
        }

        user = await _userService.Find(user.UserId);
        if (user == null)
        {
            _logger.LogInformation("Game Disconnected User [{Time}]: user^2 is null", DateTimeOffset.UtcNow);
            Context.Abort();
            return;
        }
        
        foreach (var gameUser in game.Users)
        {
            await Clients.Caller.SendAsync("NewUser", new UserDto(
                gameUser.ConnectionId!, gameUser.UserId, gameUser.Name, gameUser.ImageUrl, gameUser.DivisionId, gameUser.Health, 
                gameUser.GuessAvailable));
        }
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
            await Clients.Group(user.Game!.Code).SendAsync("RemoveUser", new RemoveUserDto(
                user.ConnectionId!, user.UserId));
        }
        await base.OnDisconnectedAsync(exception);
    }
}