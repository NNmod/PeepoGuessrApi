using PeepoGuessrApi.Entities.Databases.Maintenance;

namespace PeepoGuessrApi.Services.Interfaces.Maintenance.Db;

public interface IAccessService
{
    public Task<Access?> Find();
    public Task<bool> Update(Access access);
}