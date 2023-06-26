using PeepoGuessrApi.Entities.Request.Twitch;
using PeepoGuessrApi.Entities.Response.Http.Twitch;

namespace PeepoGuessrApi.Services.Interfaces.Twitch;

public interface IOAuth2Service
{
    public Task<OAuthTokenDto?> GetClientCredentials(OAuthTokenReqDto oAuthTokenReqDto);
}