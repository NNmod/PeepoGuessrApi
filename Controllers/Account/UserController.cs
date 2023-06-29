using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeepoGuessrApi.Entities.Databases.Account;
using PeepoGuessrApi.Entities.Databases.Maintenance;
using PeepoGuessrApi.Entities.Request.Twitch;
using PeepoGuessrApi.Entities.Response;
using PeepoGuessrApi.Services.Interfaces.Account.Db;
using PeepoGuessrApi.Services.Interfaces.Maintenance;
using PeepoGuessrApi.Services.Interfaces.Maintenance.Db;
using PeepoGuessrApi.Services.Interfaces.Twitch;
using IAuthorizationService = PeepoGuessrApi.Services.Interfaces.Maintenance.Db.IAuthorizationService;
using UserDto = PeepoGuessrApi.Entities.Response.Account.User.UserDto;

namespace PeepoGuessrApi.Controllers.Account;

[ApiController]
[Authorize]
[Route("api/account/user")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IAccessService _accessService;
    private readonly IOAuth2Service _oAuth2Service;
    private readonly IGetUsersService _getUsersService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserService _userService;
    private readonly IGameService _gameService;
    private readonly IGameStatusService _gameStatusService;
    private readonly Services.Interfaces.Game.Db.IGameService _gameGameService;

    public UserController(IConfiguration configuration, IAccessService accessService, IOAuth2Service oAuth2Service,
        IGetUsersService getUsersService, IAuthorizationService authorizationService, IUserService userService,
        IGameService gameService, IGameStatusService gameStatusService, Services.Interfaces.Game.Db.IGameService gameGameService)
    {
        _configuration = configuration;
        _accessService = accessService;
        _oAuth2Service = oAuth2Service;
        _getUsersService = getUsersService;
        _authorizationService = authorizationService;
        _userService = userService;
        _gameService = gameService;
        _gameStatusService = gameStatusService;
        _gameGameService = gameGameService;
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTop(int page = 0)
    {
        var count = await _userService.Count();
        var pages = (int)Math.Ceiling((double)count / 25);
        var items = await _userService.FindList(25, page * 25);
        return Ok(new ListDto<UserDto>
        {
            Pages = pages,
            Items = items.Select(u => new UserDto(u.Id, u.TwitchId, u.Name, u.ImageUrl, u.DivisionId, u.Score, u.Wins, 
                    null, null))
                .ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        if (!ModelState.IsValid || HttpContext.User.Identity is { Name: null } or null ||
            !int.TryParse(HttpContext.User.Identity.Name, out var userId))
            return BadRequest(new ErrorDto<object?>(400, nameof(GetUser), null));
        var user = await _userService.Find(userId);
        if (user == null)
            return NotFound(new ErrorDto<object?>(404, nameof(GetUser), null));
        var gameStatus = await _gameStatusService.Find("active");
        var game = gameStatus == null ? null : await _gameService.FindByUserAndStatus(user.Id, gameStatus);
        var gameGame = game == null ? null : await _gameGameService.Find(game.Id);
        if (DateTime.UtcNow.Subtract(user.Update).Hours > 12)
        {
            var access = await _accessService.Find();
            if (access == null)
                return NotFound(new ErrorDto<object?>(404, nameof(GetUser), null));
            if (access.Token == null || DateTime.UtcNow >= access.Expire)
            {
                var tokenDto = await _oAuth2Service.GetClientCredentials(new OAuthTokenReqDto(_configuration["Twitch:ClientId"]!, 
                    _configuration["Twitch:ClientSecret"]!, "client_credentials"));
                if (tokenDto?.Access_Token == null)
                    return StatusCode(500, new ErrorDto<object?>(500, nameof(GetUser), null));
                access.Token = tokenDto.Access_Token;
                access.Expire = DateTime.UtcNow.AddSeconds(tokenDto.Expires_In).AddMinutes(-10);
                if (!await _accessService.Update(access))
                    return StatusCode(500, new ErrorDto<object?>(500, nameof(GetUser), null));
            }
            var userDto = await _getUsersService.GetUser(user.TwitchId, _configuration["Twitch:ClientId"]!, access.Token!);
            if (userDto == null)
                return Ok(new UserDto(user.Id, user.TwitchId, user.Name, user.ImageUrl, user.DivisionId, user.Score, user.Wins,
                    gameGame?.Code, gameGame?.RoundExpire));
            user.Name = userDto.Display_Name ?? user.Name;
            user.ImageUrl = userDto.Profile_Image_Url ?? user.ImageUrl;
            user.Update = DateTime.UtcNow;
            if (await _userService.Update(user))
                user = await _userService.Find(userId);
        }
        if (user == null)
            return NotFound(new ErrorDto<object?>(404, nameof(GetUser), null));
        return Ok(new UserDto(user.Id, user.TwitchId, user.Name, user.ImageUrl, user.DivisionId, user.Score, user.Wins,
            gameGame?.Code, gameGame?.RoundExpire));
    }

    [AllowAnonymous]
    [HttpGet("sign-in/preprocessing")]
    public async Task<IActionResult> SignInPreprocessing()
    {
        if (HttpContext.User.Identity is { IsAuthenticated: true })
            return Conflict(new ErrorDto<object?>(409, nameof(SignInPreprocessing), null));
        var authorization = new Authorization
        {
            Code = Guid.NewGuid().ToString()
        };
        if (!await _authorizationService.Create(authorization))
            return StatusCode(500, new ErrorDto<object?>(500, nameof(SignInPreprocessing), null));
        return Redirect($"https://id.twitch.tv/oauth2/authorize?response_type=code&client_id={_configuration["Twitch:ClientId"]}" +
                        $"&redirect_uri={_configuration["Twitch:ClientRedirect"]}&state={authorization.Code}");
    }
    
    [AllowAnonymous]
    [HttpGet("sign-in/processing")]
    public async Task<IActionResult> SignInProcessing(string? code, string? error, string? state)
    {
        if (code == null || error != null)
        {
            var authorizationError = await _authorizationService.Find(state!);
            if (authorizationError == null)
                return Ok();
            await _authorizationService.Remove(authorizationError.Id);
            return Ok(new { code, error, state });
        }
        
        var authorization = await _authorizationService.Find(state!);
        if (authorization == null)
            return Ok();
        var result = await _authorizationService.Remove(authorization.Id);
        if (!result)
            return StatusCode(500, new ErrorDto<object?>(500, nameof(SignInProcessing), new { code, error, state }));
        var userToken = await _getUsersService.GetUserCredentials(new UserAuthTokenDto(_configuration["Twitch:ClientId"]!,
            _configuration["Twitch:ClientSecret"]!, code, "authorization_code",
            _configuration["Twitch:ClientRedirect"]!));
        if (userToken == null)
            return StatusCode(500, new ErrorDto<object?>(500, nameof(SignInProcessing), new { code, error, state }));
        var userDto = await _getUsersService.GetUser(_configuration["Twitch:ClientId"]!, userToken.Access_Token);
        if (userDto?.Id == null || userDto.Display_Name == null || userDto.Profile_Image_Url == null)
            return NotFound(new ErrorDto<object?>(404, nameof(SignInProcessing), new { code, error, state }));
        var user = await _userService.FindByTwitch(userDto.Id);
        if (user == null)
        {
            user = new User
            {
                DivisionId = 1,
                TwitchId = userDto.Id,
                Name = userDto.Display_Name,
                ImageUrl = userDto.Profile_Image_Url,
                Score = 200,
                Update = DateTime.UtcNow
            };
            if (!await _userService.Create(user))
                return StatusCode(500, new ErrorDto<object?>(500, nameof(SignInProcessing), new { code, error, state }));
            user = await _userService.FindByTwitch(userDto.Id);
            if (user == null)
                return NotFound(new ErrorDto<object?>(404, nameof(SignInProcessing), new { code, error, state }));
        }
        else
        {
            user.Name = userDto.Display_Name;
            user.ImageUrl = userDto.Profile_Image_Url;
            user.Update = DateTime.UtcNow;
            if (!await _userService.Update(user))
                return StatusCode(500, new ErrorDto<object?>(500, nameof(SignInProcessing), new { code, error, state })); 
        }
        var claims = new List<Claim> { 
            new(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString())
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(new ClaimsIdentity(claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType)));
        return Redirect("https://ppg.nnmod.com/close.html");
    }

    [HttpGet("sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

}