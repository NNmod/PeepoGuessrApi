using PeepoGuessrApi.Entities.Databases.Game;

namespace PeepoGuessrApi.Services.Interfaces.Game.Db;

public interface IGameTypeService
{
    public Task<GameType?> Find(string name);
    public Task<GameType?> FindInclude(string name);
}