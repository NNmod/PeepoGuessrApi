using PeepoGuessrApi.Entities.Databases.Maintenance;

namespace PeepoGuessrApi.Services.Interfaces.Maintenance.Db;

public interface IWorkService
{
    public Task<Work?> FindActive();
}