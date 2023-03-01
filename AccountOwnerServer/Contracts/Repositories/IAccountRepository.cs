using Entities.Models;

namespace Contracts.Repositories;

public interface IAccountRepository : IRepositoryBase<Account>
{
    IEnumerable<Account> AccountsByOwner(Guid ownerId);
}
