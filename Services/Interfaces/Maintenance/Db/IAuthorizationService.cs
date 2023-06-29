using PeepoGuessrApi.Entities.Databases.Maintenance;

namespace PeepoGuessrApi.Services.Interfaces.Maintenance.Db;

public interface IAuthorizationService
{
    public Task<Authorization?> Find(string code);
    public Task<bool> Create(Authorization authorization);
    public Task<bool> Remove(int id);
}