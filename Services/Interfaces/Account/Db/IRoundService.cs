using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IRoundService
{
    public Task<Round?> Find(int id);
    public Task<Round?> Find(Entities.Databases.Account.Game game, int count);
    public Task<bool> Create(Round round);
}