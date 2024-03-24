using ArbTech.SharedKernel.Messaging;

namespace ArbTech.SharedKernel.Interfaces;

public interface IIntegrationRepository<T> : IRepository<T>
    where T : class, IBaseEntity, IAggregateRoot
{
    Task<T> AddAsync(T entity, Func<BaseIntegrationEvent?> eventBuilder, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, Func<BaseIntegrationEvent?> eventBuilder, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(T entity, Func<BaseIntegrationEvent?> eventBuilder, CancellationToken cancellationToken = default);
    
    Task UpdateRangeAsync(IEnumerable<T> entities, Func<BaseIntegrationEvent?> eventBuilder, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(T entity, Func<BaseIntegrationEvent?> eventBuilder, CancellationToken cancellationToken = default);
    
    Task DeleteRangeAsync(IEnumerable<T> entities, Func<BaseIntegrationEvent?> eventBuilder, CancellationToken cancellationToken = default);
    
    Task<int> SaveChangesAsync(Func<BaseIntegrationEvent?> eventBuilder, CancellationToken cancellationToken = default);
}
