using PeepoGuessrApi.Entities.Databases.Lobby;

namespace PeepoGuessrApi.Services.Interfaces.LobbyDb;

public interface ILobbyTypeService
{
    public Task<LobbyType?> Find(string name);
    public Task<LobbyType?> FindInclude(string name);
}