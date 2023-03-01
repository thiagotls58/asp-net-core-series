namespace Contracts.Repositories;

public interface IRepositoryWrapper
{
    IOwnerRepository Owner { get; }
    IAccountRepository Account { get; }
    void Save();
}
