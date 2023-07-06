using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PeepoGuessrApi.Entities.Databases.Lobby;
using PeepoGuessrApi.Entities.Request.Lobby;
using PeepoGuessrApi.Entities.Response;
using PeepoGuessrApi.Entities.Response.Hubs.Lobby;
using PeepoGuessrApi.Hubs;
using PeepoGuessrApi.Services.Interfaces.Lobby.Db;

namespace PeepoGuessrApi.Controllers.Lobby;

[ApiController]
[Authorize]
[Route("api/lobby/lobby")]
public class LobbyController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserInviteService _userInviteService;
    private readonly IHubContext<LobbyHub> _lobbyHubContext;

    public LobbyController(IUserService userService, IUserInviteService userInviteService, IHubContext<LobbyHub> lobbyHubContext)
    {
        _userService = userService;
        _userInviteService = userInviteService;
        _lobbyHubContext = lobbyHubContext;
    }

    [HttpPost("random")]
    public async Task<IActionResult> UpdateRandom(RandomDto randomDto)
    {
        if (!ModelState.IsValid || HttpContext.User.Identity is { Name: null } or null ||
            !int.TryParse(HttpContext.User.Identity.Name, out var userId))
            return BadRequest(new ErrorDto<RandomDto>(400, nameof(UpdateRandom), randomDto));
        
        var user = await _userService.Find(userId);
        if (user == null)
            return NotFound(new ErrorDto<RandomDto>(404, nameof(UpdateRandom), randomDto));

        user.IsRandomAcceptable = randomDto.IsRandomAcceptable;

        if (await _userService.Update(user))
        {
            await _lobbyHubContext.Clients.GroupExcept(user.LobbyType?.Name.ToLower() + '-' + user.DivisionId, 
                user.ConnectionId).SendAsync("RandomUpdate", new UserRandomDto(user.UserId, user.IsRandomAcceptable));
            return Ok();
        }
        return StatusCode(500, new ErrorDto<RandomDto>(500, nameof(UpdateRandom), randomDto));
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InvitePlayer(InviteDto inviteDto)
    {
        if (!ModelState.IsValid || HttpContext.User.Identity is { Name: null } or null ||
            !int.TryParse(HttpContext.User.Identity.Name, out var userId))
            return BadRequest(new ErrorDto<InviteDto>(400, nameof(InvitePlayer), inviteDto));
        
        var user = await _userService.Find(userId);
        if (user == null)
            return NotFound(new ErrorDto<InviteDto>(404, nameof(InvitePlayer), inviteDto));

        var invitedUser = await _userService.Find(inviteDto.UserId);
        if (invitedUser == null)
            return NotFound(new ErrorDto<InviteDto>(404, nameof(InvitePlayer), inviteDto));
        if (user.UserInvites.Any(u => u.RequestedUserId == invitedUser.UserId))
            return Conflict(new ErrorDto<InviteDto>(409, nameof(InvitePlayer), inviteDto));

        var userInvite = new UserInvite
        {
            UserId = user.UserId,
            RequestedUserId = invitedUser.UserId
        };
        if (await _userInviteService.Create(userInvite))
        {
            await _lobbyHubContext.Clients.Client(user.ConnectionId).SendAsync("InviteAdded", new UserInviteDto(
                invitedUser.UserId));
            await _lobbyHubContext.Clients.Client(invitedUser.ConnectionId).SendAsync("InviteAchieve", new UserInviteDto(
                user.UserId));
            return Ok();
        }
        return StatusCode(500, new ErrorDto<InviteDto>(500, nameof(InvitePlayer), inviteDto));
    }

    [HttpPost("invite/remove")]
    public async Task<IActionResult> RemoveInvitePlayer(InviteDto inviteDto)
    {
        if (!ModelState.IsValid || HttpContext.User.Identity is { Name: null } or null ||
            !int.TryParse(HttpContext.User.Identity.Name, out var userId))
            return BadRequest(new ErrorDto<InviteDto>(400, nameof(RemoveInvitePlayer), inviteDto));
        
        var user = await _userService.Find(userId);
        if (user == null)
            return NotFound(new ErrorDto<InviteDto>(404, nameof(RemoveInvitePlayer), inviteDto));

        var userInvite = user.UserInvites.FirstOrDefault(u => u.RequestedUserId == inviteDto.UserId);
        if (userInvite == null)
            return NotFound(new ErrorDto<InviteDto>(404, nameof(RemoveInvitePlayer), inviteDto));
        var invitedUser = await _userService.Find(userInvite.UserId);
        if (invitedUser == null)
            return NotFound(new ErrorDto<InviteDto>(404, nameof(RemoveInvitePlayer), inviteDto));
        
        if (await _userInviteService.Remove(invitedUser.Id))
        {
            await _lobbyHubContext.Clients.Client(user.ConnectionId).SendAsync("InviteRemoved", new UserInviteDto(
                invitedUser.UserId));
            await _lobbyHubContext.Clients.Client(invitedUser.ConnectionId).SendAsync("InviteRevoked", new UserInviteDto(
                user.UserId));
            return Ok();
        }
        return StatusCode(500, new ErrorDto<InviteDto>(500, nameof(InvitePlayer), inviteDto)); 
    }
}