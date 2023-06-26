using PeepoGuessrApi.Entities.Request.Twitch;
using PeepoGuessrApi.Entities.Response.Http.Twitch;
using PeepoGuessrApi.Services.Interfaces.Twitch;

namespace PeepoGuessrApi.Services.Implementations.Twitch;

public class OAuth2Service : IOAuth2Service
{
    private readonly HttpClient _httpClient;
    
    public OAuth2Service(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<OAuthTokenDto?> GetClientCredentials(OAuthTokenReqDto oAuthTokenReqDto)
    {
        try
        {
            var values = new Dictionary<string, string>
            {
                { "client_id", oAuthTokenReqDto.Client_Id },
                { "client_secret", oAuthTokenReqDto.Client_Secret },
                { "grant_type", oAuthTokenReqDto.Grant_Type }
            };
            var tokenReq = await _httpClient.PostAsync("https://id.twitch.tv/oauth2/token", 
                new FormUrlEncodedContent(values));
            if (!tokenReq.IsSuccessStatusCode)
                return null;
            return await tokenReq.Content.ReadFromJsonAsync<OAuthTokenDto>();
        }
        catch
        {
            return null;
        }
    }
}