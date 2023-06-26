using PeepoGuessrApi.Entities.Databases.Lobby;

namespace PeepoGuessrApi.Services.Interfaces.LobbyDb;

public interface IUserService
{
    public Task<User?> Find(int userId);
    public Task<bool> Create(User user);
    public Task<bool> Update(User user);
    public Task<bool> Remove(int id);
}