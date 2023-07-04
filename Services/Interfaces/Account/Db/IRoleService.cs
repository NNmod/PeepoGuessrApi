using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IRoleService
{
    public Task<Role?> Find(string name);
}