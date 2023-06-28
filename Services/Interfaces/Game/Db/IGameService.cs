namespace PeepoGuessrApi.Services.Interfaces.Game.Db;

public interface IGameService
{
    public Task<int> Count();
    public Task<Entities.Databases.Game.Game?> Find(int gameId);
    public Task<Entities.Databases.Game.Game?> FindByCode(string code);
    public Task<bool> Create(Entities.Databases.Game.Game game);
    public Task<bool> Update(Entities.Databases.Game.Game game);
    public Task<bool> Remove(int id);
}