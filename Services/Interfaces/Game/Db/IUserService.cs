using PeepoGuessrApi.Entities.Databases.Game;

namespace PeepoGuessrApi.Services.Interfaces.Game.Db;

public interface IUserService
{
    public Task<User?> Find(int userId);
    public Task<bool> Create(User user);
    public Task<bool> Update(User user);
    public Task<bool> Remove(int id);
}