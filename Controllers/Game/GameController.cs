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
    private readonly IGameService _gameService;
    private readonly IUserService _userService;

    public GameController(IHubContext<GameHub> gameHubContext, IGameService gameService, IUserService userService)
    {
        _gameHubContext = gameHubContext;
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
        if (user == null || user.GuessAvailable == 0)
            return NotFound(new ErrorDto<GuessReqDto>(404, nameof(Guess), guessReqDto));
        var game = await _gameService.Find(user.GameId);
        if (game == null)
            return NotFound(new ErrorDto<GuessReqDto>(404, nameof(Guess), guessReqDto));
        user.PosX = guessReqDto.PosX;
        user.PosY = guessReqDto.PosY;
        user.Distance = Math.Sqrt(Math.Pow(guessReqDto.PosX - game.PosX, 2) + Math.Pow(guessReqDto.PosY - game.PosY, 2));
        user.GuessAvailable--;
        if (!await _userService.Update(user))
            return StatusCode(500, new ErrorDto<GuessReqDto>(500, nameof(Guess), guessReqDto));
        
        await _gameHubContext.Clients.Group(game.Code).SendAsync("UserGuess", new GuessDto(user.UserId, user.GuessAvailable, user.Distance));
        
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