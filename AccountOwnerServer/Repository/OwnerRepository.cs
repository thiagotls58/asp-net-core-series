using Contracts.Repositories;
using Entities.Helpers;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
{
    private readonly RepositoryContext _context;
    private readonly ISortHelper<Owner> _sortHelper;

    public OwnerRepository(RepositoryContext context, 
        ISortHelper<Owner> sortHelper) : base(context)
    {
        _context = context;
        _sortHelper = sortHelper;
    }

    public void CreateOwner(Owner owner) => Create(owner);

    public void DeleteOwner(Owner owner) => Delete(owner);

    public IEnumerable<Owner> GetAllOwners() => FindAll().OrderBy(o => o.Name).ToList();

    public Owner GetOwnerById(Guid Id) => FindByCondition(o => o.Id.Equals(Id)).FirstOrDefault();

    public PagedList<Owner> GetOwners(OwnerParameters ownerParameters)
    {
        var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);

        SearchByName(ref owners, 
            ownerParameters.Name);

        owners = _sortHelper.ApplySort(owners, 
            ownerParameters.OrderBy);

        return PagedList<Owner>.ToPagedList(owners.OrderBy(o => o.Name),
            ownerParameters.PageNumber,
            ownerParameters.PageSize);
    }

    private void SearchByName(ref IQueryable<Owner> owners, 
        string ownerName)
    {
        if (string.IsNullOrWhiteSpace(ownerName))
            return;

        owners.Where(o => o.Name.ToLower().Contains(ownerName.ToLower()));
    }

    public Owner GetOwnerWithDetails(Guid Id) => FindByCondition(o => o.Id.Equals(Id))
        .Include(o => o.Accounts)
        .FirstOrDefault();

    public void UpdateOwner(Owner owner) => Update(owner);
}
