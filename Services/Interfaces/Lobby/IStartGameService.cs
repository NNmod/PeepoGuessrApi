using PeepoGuessrApi.Entities.Databases.Lobby;
using User = PeepoGuessrApi.Entities.Databases.Lobby.User;

namespace PeepoGuessrApi.Services.Interfaces.Lobby;

public interface IStartGameService
{
    public Task<bool> StartSingleGame(User user, string gameTypeName, string code);
    public Task<bool> StartMultiplayerGame(User user1, User user2, string gameTypeName, string code);
}