namespace ArbTech.SharedKernel.Interfaces;

// from Ardalis.Specification
public interface IRepository<T> : IReadRepository<T>
    where T : class, IBaseEntity, IAggregateRoot
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    [Obsolete("The update method is unnecessary in DDD application architecture", true)]
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    [Obsolete("The update method is unnecessary in DDD application architecture", true)]
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
}
