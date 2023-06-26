using PeepoGuessrApi.Entities.Request.Twitch;
using PeepoGuessrApi.Entities.Response.Http.Twitch;

namespace PeepoGuessrApi.Services.Interfaces.Twitch;

public interface IGetUsersService
{
    public Task<OAuthTokenDto?> GetUserCredentials(UserAuthTokenDto userAuthTokenDto);
    public Task<UserDto?> GetUser(string clientId, string bearerToken);
    public Task<UserDto?> GetUser(string id, string clientId, string bearerToken);
}