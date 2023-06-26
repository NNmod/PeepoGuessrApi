using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IGameStatusService
{
    public Task<GameStatus?> Find(string name);
}