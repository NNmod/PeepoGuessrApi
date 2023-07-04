using Microsoft.EntityFrameworkCore;
using PeepoGuessrApi.Databases;
using PeepoGuessrApi.Entities.Databases.Maintenance;
using PeepoGuessrApi.Services.Interfaces.Maintenance.Db;

namespace PeepoGuessrApi.Services.Implementations.Maintenance.Db;

public class WorkService : IWorkService
{
    private readonly IDbContextFactory<MaintenanceDbContext> _maintenanceDbContextFactory;

    public WorkService(IDbContextFactory<MaintenanceDbContext> maintenanceDbContextFactory)
    {
        _maintenanceDbContextFactory = maintenanceDbContextFactory;
    }

    public async Task<Work?> FindActive()
    {
        await using var context = await _maintenanceDbContextFactory.CreateDbContextAsync();
        return await context.Works.Where(t => t.Expire >= DateTime.UtcNow).FirstOrDefaultAsync();
    }
}