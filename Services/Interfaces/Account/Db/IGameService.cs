using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IGameService
{
    public Task<Entities.Databases.Account.Game?> Find(int id);
    public Task<Entities.Databases.Account.Game?> FindByCode(string code);
    public Task<Entities.Databases.Account.Game?> FindByUserAndStatus(int userId, GameStatus gameStatus);
    public Task<bool> Create(Entities.Databases.Account.Game game);
    public Task<bool> Update(Entities.Databases.Account.Game game);
}