using Contracts.Repositories;
using Entities.Helpers;
using Entities.Models;

namespace Repository;

public class AccountRepository : RepositoryBase<Account>, IAccountRepository
{
    private readonly RepositoryContext _context;
    private readonly ISortHelper<Account> _sortHelper;

    public AccountRepository(RepositoryContext context, 
        ISortHelper<Account> sortHelper) : base(context)
    {
        _context = context;
        _sortHelper = sortHelper;
    }

    public IEnumerable<Account> AccountsByOwner(Guid ownerId) => FindByCondition(a => a.OwnerId == ownerId);
}
