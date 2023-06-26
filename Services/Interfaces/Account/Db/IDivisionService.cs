using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IDivisionService
{
    public Task<List<Division>> FindAll();
    public Task<Division?> Find(string name);
    public Task<Division?> FindByScore(int score);
}