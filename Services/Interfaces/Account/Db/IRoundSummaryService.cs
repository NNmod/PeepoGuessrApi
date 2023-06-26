using PeepoGuessrApi.Entities.Databases.Account;

namespace PeepoGuessrApi.Services.Interfaces.Account.Db;

public interface IRoundSummaryService
{
    public Task<bool> Create(RoundSummary roundSummary);
    public Task<bool> Update(RoundSummary roundSummary);
}