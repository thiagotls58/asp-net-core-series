using Contracts.Repositories;
using Entities.Helpers;
using Entities.Models;

namespace Repository;

public class RepositoryWrapper : IRepositoryWrapper
{
    private RepositoryContext _context;
    private IOwnerRepository _owner;
    private IAccountRepository _account;
    private ISortHelper<Owner> _ownerSortHelper;
    private ISortHelper<Account> _accountSortHelper;

    public IOwnerRepository Owner
    {
        get
        {
            if (_owner == null)
                _owner = new OwnerRepository(_context,
                    _ownerSortHelper);
            return _owner;
        }
    }

    public IAccountRepository Account
    {
        get
        {
            if (_account == null) 
                _account = new AccountRepository(_context,
                    _accountSortHelper);
            return _account;
        }
    }

    public RepositoryWrapper(RepositoryContext context, 
        ISortHelper<Owner> ownerSortHelper, 
        ISortHelper<Account> accountSortHelper)
    {
        _context = context;
        _ownerSortHelper = ownerSortHelper;
        _accountSortHelper = accountSortHelper;
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
