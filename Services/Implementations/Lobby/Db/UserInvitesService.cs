using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Lobby;
using PeepoGuessrApi.Services.Interfaces.Lobby.Db;

namespace PeepoGuessrApi.Services.Implementations.Lobby.Db;

public class UserInvitesService : IUserInviteService
{
    private readonly IDbContextFactory<LobbyDbContext> _lobbyDbContextFactory;

    public UserInvitesService(IDbContextFactory<LobbyDbContext> lobbyDbContextFactory)
    {
        _lobbyDbContextFactory = lobbyDbContextFactory;
    }

    public async Task<bool> Create(UserInvite userInvite)
    {
        var addUserInvite = new UserInvite
        {
            UserId = userInvite.UserId,
            InvitedUserId = userInvite.InvitedUserId
        };
        await using var context = await _lobbyDbContextFactory.CreateDbContextAsync();
        await context.AddAsync(addUserInvite);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Remove(int id)
    {
        await using var context = await _lobbyDbContextFactory.CreateDbContextAsync();
        var userInvite = await context.UserInvites.FirstOrDefaultAsync(ui => ui.Id == id);
        if (userInvite == null)
            return false;
        context.Remove(userInvite);
        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}