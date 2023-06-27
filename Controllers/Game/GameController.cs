using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Entities.Request.Game;
using PeepoGuessrApi.Entities.Response;
using PeepoGuessrApi.Entities.Response.Hubs.Game;
using PeepoGuessrApi.Hubs;
using PeepoGuessrApi.Services.Interfaces.Game.Db;

namespace PeepoGuessrApi.Controllers.Game;

[ApiController]
[Authorize]
[Route("api/game/game")]
public class GameController : ControllerBase
{
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly Services.Interfaces.Account.Db.IGameService _accountGameService;
    private readonly Services.Interfaces.Account.Db.IRoundService _accountRoundService;
    private readonly Services.Interfaces.Account.Db.IRoundSummaryService _accountRoundSummaryService;
    private readonly IGameService _gameService;
    private readonly IUserService _userService;

    public GameController(IHubContext<GameHub> gameHubContext, Services.Interfaces.Account.Db.IGameService accountGameService,
        Services.Interfaces.Account.Db.IRoundService accountRoundService, 
        Services.Interfaces.Account.Db.IRoundSummaryService accountRoundSummaryService,
        IGameService gameService, IUserService userService)
    {
        _gameHubContext = gameHubContext;
        _accountGameService = accountGameService;
        _accountRoundService = accountRoundService;
        _accountRoundSummaryService = accountRoundSummaryService;
        _gameService = gameService;
        _userService = userService;
    }
    
    [HttpPost("guess")]
    public async Task<IActionResult> Guess(GuessReqDto guessReqDto)
    {
        if (!ModelState.IsValid || HttpContext.User.Identity is { Name: null } or null ||
            !int.TryParse(HttpContext.User.Identity.Name, out var userId))
            return BadRequest(new ErrorDto<GuessReqDto>(400, nameof(Guess), guessReqDto));
        
        var user = await _userService.Find(userId);
        if (user == null)
            return NotFound(new ErrorDto<GuessReqDto>(404, nameof(Guess), guessReqDto));
        
        var game = await _gameService.Find(user.GameId);
        if (game == null)
            return NotFound(new ErrorDto<GuessReqDto>(404, nameof(Guess), guessReqDto));
        
        var accountGame = await _accountGameService.Find(game.GameId);
        if (accountGame == null)
            return NotFound(new ErrorDto<GuessReqDto>(404, nameof(Guess), guessReqDto));
        
        var accountRound = await _accountRoundService.Find(accountGame, game.RoundCount);
        if (accountRound == null)
            return NotFound(new ErrorDto<GuessReqDto>(404, nameof(Guess), guessReqDto));
        
        var roundUser = accountRound.RoundSummaries.FirstOrDefault(u => u.UserId == user.UserId);
        if (roundUser == null)
            return NotFound(new ErrorDto<GuessReqDto>(404, nameof(Guess), guessReqDto));
        
        roundUser.PosX = guessReqDto.PosX;
        roundUser.PosY = guessReqDto.PosY;
        roundUser.Distance = Math.Sqrt(Math.Pow(guessReqDto.PosX - game.PosX, 2) + Math.Pow(guessReqDto.PosY - game.PosY, 2));
        roundUser.GuessAvailable--;
        if (!await _accountRoundSummaryService.Update(roundUser))
            return StatusCode(500, new ErrorDto<GuessReqDto>(500, nameof(Guess), guessReqDto));
        
        await _gameHubContext.Clients.Group(game.Code).SendAsync("UserGuess", new GuessDto(roundUser.UserId, 
            roundUser.GuessAvailable, roundUser.Distance));
        
        if (game is { GameType.IsPromotionEnable: true, IsRoundPromoted: false })
        {
            var roundExpire = DateTime.UtcNow.AddSeconds(game.GameType.RoundPromotionDuration);
            if (game.RoundExpire > roundExpire)
                game.RoundExpire = roundExpire;
            game.IsRoundPromoted = true;
            if (!await _gameService.Update(game))
                return StatusCode(500, new ErrorDto<GuessReqDto>(500, nameof(Guess), guessReqDto));
            
            await _gameHubContext.Clients.Group(game.Code).SendAsync("RoundPromotion", new RoundPromotionDto(game.RoundExpire));
        }

        return Ok();
    }
}