using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface ISummaryService
{
    public Task<int> Count(int userId);
    public Task<List<Summary>> FindList(int userId, int take, int skip = 0);
    public Task<Summary?> Find(int id);
    public Task<bool> Create(Summary summary);
    public Task<bool> Update(Summary summary);
}