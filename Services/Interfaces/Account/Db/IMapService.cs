using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IMapService
{
    public Task<List<Map>> FindAll();
    public Task<List<Map>> FindClassic();
    public Task<Map?> Find(string name);
}