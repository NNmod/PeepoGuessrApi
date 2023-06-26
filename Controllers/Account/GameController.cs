using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeepoGuessrApi.Entities.Response;
using PeepoGuessrApi.Entities.Response.Account.Game;
using PeepoGuessrApi.Entities.Response.Account.User;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Controllers.Account;

[ApiController]
[Authorize]
[Route("api/account/game")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGame(string code)
    {
        var game = await _gameService.FindByCode(code);
        if (game == null)
            return NotFound(new ErrorDto<object>(404, nameof(GetGame), code));
        return Ok(new GameDto(game.Id, game.Code, game.GameType!.Name, game.GameStatus!.Name,
            game.DateTime, game.Rounds.Select(r => new GameRoundDto(r.Id, r.Map!.Name, r.Count)).ToList(), 
            game.Summaries.Select(s => new UserDto(s.User!.Id, s.User.TwitchId, s.User.Name, s.User.ImageUrl, 
                s.DivisionId, s.Score, s.User.Wins)).ToList()));
    }
}