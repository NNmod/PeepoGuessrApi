using PeepoGuessrApi.Entities;

namespace PeepoGuessrApi.Services.Interfaces.Game;

public interface IMapService
{
    public Task<Point?> RandomPosition(string clientCdnLink, string mapName);
}