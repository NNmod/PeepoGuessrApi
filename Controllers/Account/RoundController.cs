using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeepoGuessrApi.Entities.Response;
using PeepoGuessrApi.Entities.Response.Account.Round;
using PeepoGuessrApi.Services.Interfaces.Account.Db;

namespace PeepoGuessrApi.Controllers.Account;

[ApiController]
[Authorize]
[Route("api/account/round/summary")]
public class RoundController : ControllerBase
{
    private readonly IRoundService _roundService;

    public RoundController(IRoundService roundService)
    {
        _roundService = roundService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRound(int id)
    {
        var round = await _roundService.Find(id);
        if (round == null)
            return NotFound(new ErrorDto<object>(404, nameof(GetRound), id));
        return Ok(new RoundDto(round.Id, round.Map!.Name, round.Count, round.PosX, round.PosY,
            round.RoundSummaries.Select(rs => new RoundUserDto(rs.User!.Id, rs.User.TwitchId, rs.User.Name,
                rs.User.ImageUrl, rs.Health, rs.Damage, rs.Distance, rs.PosX, rs.PosY)).ToList()));
    }
}