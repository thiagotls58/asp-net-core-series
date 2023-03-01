using Entities.Models;

namespace Contracts.Repositories;

public interface IOwnerRepository : IRepositoryBase<Owner>
{
    IEnumerable<Owner> GetAllOwners();
    Owner GetOwnerById(Guid Id);
    Owner GetOwnerWithDetails(Guid Id);
    void CreateOwner(Owner owner);
    void UpdateOwner(Owner owner);
    void DeleteOwner(Owner owner);
    PagedList<Owner> GetOwners(OwnerParameters ownerParameters);
}
