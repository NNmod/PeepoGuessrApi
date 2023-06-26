using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IUserService
{
    public Task<int> Count();
    public Task<List<User>> FindList(int take, int skip = 0);
    public Task<User?> Find(int id);
    public Task<User?> FindByTwitch(string twitchId);
    public Task<bool> Create(User user);
    public Task<bool> Update(User user);
}