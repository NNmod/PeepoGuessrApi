using PeepoGuessrApi.Entities;
using PeepoGuessrApi.Entities.Response.Http.Cdn;
using PeepoGuessrApi.Services.Interfaces.Game;

namespace PeepoGuessrApi.Services.Implementations.Game;

public class MapService : IMapService
{
    private readonly HttpClient _httpClient;

    public MapService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Point?> RandomPosition(string clientCdnLink, string mapName)
    {
        try
        {
            var tokenReq = await _httpClient.GetAsync($"{clientCdnLink}/{mapName}/settings.json");
            if (!tokenReq.IsSuccessStatusCode)
                return null;
            
            var mapSettings = await tokenReq.Content.ReadFromJsonAsync<MapSettingsDto>();
            if (mapSettings == null)
                return null;
            
            var minPosX = -4000;
            var minPosY = -4000;
            var maxPosX = 4000;
            var maxPosY = 4000;
            
            if (mapSettings.MinPos.Count > 2)
            {
                minPosX = mapSettings.MinPos[0];
                minPosY = mapSettings.MinPos[1];
            }
            if (mapSettings.MaxPos.Count > 2)
            {
                maxPosX = mapSettings.MaxPos[0];
                maxPosY = mapSettings.MaxPos[1];
            }

            var rnd = new Random();
            var posX = rnd.Next((int)(minPosX + Math.Abs(minPosX * 0.125)),
                (int)(maxPosX + 1 - Math.Abs(maxPosX * 0.125)));
            var posY = rnd.Next((int)(minPosY + Math.Abs(minPosY * 0.125)),
                (int)(maxPosY + 1 - Math.Abs(maxPosY * 0.125)));

            return new Point
            {
                X = posX,
                Y = posY
            };
        }
        catch
        {
            return null;
        }
    }
}