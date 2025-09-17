namespace ShopFlow.Application.Abstractions.Mappings;

public interface IDomainPersistenceMapper<TDomain, TPersistence>
    where TDomain : class
    where TPersistence : class
{
    TDomain ToDomain(TPersistence persistenceEntity);
    TPersistence ToPersistence(TDomain domainEntity);
    void UpdatePersistence(TDomain domainEntity, TPersistence persistenceEntity);
}
