using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IGameTypeService
{
    public Task<GameType?> Find(string name);
}