using PeepoGuessrApi.Entities.Databases.Lobby;

namespace PeepoGuessrApi.Services.Interfaces.Lobby.Db;

public interface IUserInviteService
{
    public Task<bool> Create(UserInvite userInvite);
    public Task<bool> Remove(int id);
}