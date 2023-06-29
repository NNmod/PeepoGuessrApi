using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Services.Interfaces.Account.Db;
using PeepoGuessrApi.Services.Interfaces.Lobby;
using IUserService = PeepoGuessrApi.Services.Interfaces.Lobby.Db.IUserService;
using User = PeepoGuessrApi.Entities.Databases.Lobby.User;


namespace PeepoGuessrApi.Services.Implementations.Lobby;

public class StartGameService : IStartGameService
{
    private readonly IUserService _userService;
    private readonly IGameStatusService _accountGameStatusService;
    private readonly IGameTypeService _accountGameTypeService;
    private readonly IGameService _accountGameService;
    private readonly ISummaryService _accountSummaryService;
    private readonly Interfaces.Game.Db.IGameTypeService _gameGameTypeService;
    private readonly Interfaces.Game.Db.IGameService _gameGameService;

    public StartGameService(IUserService userService, IGameStatusService accountGameStatusService, IGameTypeService accountGameTypeService,
        IGameService accountGameService, ISummaryService accountSummaryService, Interfaces.Game.Db.IGameTypeService gameGameTypeService, 
        Interfaces.Game.Db.IGameService gameGameService)
    {
        _userService = userService;
        _accountGameStatusService = accountGameStatusService;
        _accountGameTypeService = accountGameTypeService;
        _accountGameService = accountGameService;
        _accountSummaryService = accountSummaryService;
        _gameGameTypeService = gameGameTypeService;
        _gameGameService = gameGameService;
    }
    
    public async Task<bool> StartSingleGame(User user, string gameTypeName, string code)
    {
        var accountGameStatus = await _accountGameStatusService.Find("active");
        if (accountGameStatus == null)
            return false;
        
        var accountGameType = await _accountGameTypeService.Find(gameTypeName);
        if (accountGameType == null)
            return false;
        
        var gameGameType = await _gameGameTypeService.Find(gameTypeName);
        if (gameGameType == null)
            return false;
                
        var accountGame = new Entities.Databases.Account.Game
        {
            GameTypeId = accountGameType.Id,
            GameStatusId = accountGameStatus.Id,
            Code = code,
            DateTime = DateTime.UtcNow
        };
        if (!await _accountGameService.Create(accountGame))
            return false;
        
        accountGame = await _accountGameService.FindByCode(accountGame.Code);
        if (accountGame == null)
            return false;
                
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
            return false;
        }

        var userSummary = new Summary
        {
            GameId = game.GameId,
            UserId = user.UserId,
            Score = user.Score
        };
        if (!await _accountSummaryService.Create(userSummary))
            return false;

        user.IsGameFounded = true;
        return await _userService.Update(user);
    }

    public async Task<bool> StartMultiplayerGame(User user1, User user2, string gameTypeName, string code)
    {
        var accountGameStatus = await _accountGameStatusService.Find("active");
        if (accountGameStatus == null)
            return false;
        
        var accountGameType = await _accountGameTypeService.Find(gameTypeName);
        if (accountGameType == null)
            return false;
        
        var gameGameType = await _gameGameTypeService.Find(gameTypeName);
        if (gameGameType == null)
            return false;
                
        var accountGame = new Entities.Databases.Account.Game
        {
            GameTypeId = accountGameType.Id,
            GameStatusId = accountGameStatus.Id,
            Code = code,
            DateTime = DateTime.UtcNow
        };
        if (!await _accountGameService.Create(accountGame))
            return false;
        
        accountGame = await _accountGameService.FindByCode(accountGame.Code);
        if (accountGame == null)
            return false;
                
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
        if (!await _gameGameService.Create(game))
        {
            accountGame.GameStatusId = 2;
            await _accountGameService.Update(accountGame);
            return false;
        }

        var user1Summary = new Summary
        {
            GameId = game.GameId,
            UserId = user1.UserId,
            Score = user1.Score
        };
        var user2Summary = new Summary
        {
            GameId = game.GameId,
            UserId = user2.UserId,
            Score = user2.Score
        };
        if (!await _accountSummaryService.Create(user1Summary) || !await _accountSummaryService.Create(user2Summary))
            return false;

        user1.IsGameFounded = true;
        user2.IsGameFounded = true;
        return await _userService.Update(user1) && await _userService.Update(user2);
    }
}