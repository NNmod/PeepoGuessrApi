namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IGameService
{
    public Task<Entities.Databases.Account.Game?> Find(int id);
    public Task<Entities.Databases.Account.Game?> FindByCode(string code);
    public Task<bool> Create(Entities.Databases.Account.Game game);
    public Task<bool> Update(Entities.Databases.Account.Game game);
}