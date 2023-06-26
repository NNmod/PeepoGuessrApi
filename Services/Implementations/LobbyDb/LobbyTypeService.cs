using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Lobby;
using PeepoGuessrApi.Services.Interfaces.LobbyDb;

namespace PeepoGuessrApi.Services.Implementations.LobbyDb;

public class LobbyTypeService : ILobbyTypeService
{
    private readonly IDbContextFactory<LobbyDbContext> _lobbyDbContextFactory;

    public LobbyTypeService(IDbContextFactory<LobbyDbContext> lobbyDbContextFactory)
    {
        _lobbyDbContextFactory = lobbyDbContextFactory;
    }
    
    public async Task<LobbyType?> Find(string name)
    {
        await using var context = await _lobbyDbContextFactory.CreateDbContextAsync();
        return await context.LobbyTypes
            .FirstOrDefaultAsync(lt => string.Equals(lt.Name.ToLower(), name.ToLower()));
    }

    public async Task<LobbyType?> FindInclude(string name)
    {
        await using var context = await _lobbyDbContextFactory.CreateDbContextAsync();
        return await context.LobbyTypes
            .Include(u => u.Users)
            .FirstOrDefaultAsync(lt => string.Equals(lt.Name.ToLower(), name.ToLower()));
    }
}