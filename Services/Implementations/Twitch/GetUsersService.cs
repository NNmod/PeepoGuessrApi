using System.Net.Http.Headers;
using PeepoGuessrApi.Entities.Request.Twitch;
using PeepoGuessrApi.Entities.Response.Http.Twitch;
using PeepoGuessrApi.Services.Interfaces.Twitch;

namespace PeepoGuessrApi.Services.Implementations.Twitch;

public class GetUsersService : IGetUsersService
{
    private readonly HttpClient _httpClient;

    public GetUsersService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OAuthTokenDto?> GetUserCredentials(UserAuthTokenDto userAuthTokenDto)
    {
        try
        {
            var values = new Dictionary<string, string>
            {
                { "client_id", userAuthTokenDto.Client_Id },
                { "client_secret", userAuthTokenDto.Client_Secret },
                { "grant_type", userAuthTokenDto.Grant_Type },
                { "redirect_uri", userAuthTokenDto.Redirect_Uri },
                { "code", userAuthTokenDto.Code }
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

    public async Task<UserDto?> GetUser(string clientId, string bearerToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("Client-Id", clientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var getUsersReq = await _httpClient.GetAsync("https://api.twitch.tv/helix/users");
            if (!getUsersReq.IsSuccessStatusCode)
                return null;
            var getUsersDto = await getUsersReq.Content.ReadFromJsonAsync<GetUsersDto>();
            if (getUsersDto == null)
                return null;
            return getUsersDto.Data.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserDto?> GetUser(string id, string clientId, string bearerToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("Client-Id", clientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var getUsersReq = await _httpClient.GetAsync($"https://api.twitch.tv/helix/users?id={id}");
            if (!getUsersReq.IsSuccessStatusCode)
                return null;
            var getUsersDto = await getUsersReq.Content.ReadFromJsonAsync<GetUsersDto>();
            return getUsersDto?.Data.FirstOrDefault(u => u.Id == id);
        }
        catch
        {
            return null;
        }
    }
}