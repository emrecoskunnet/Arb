using System.Security.Authentication;
using System.Security.Claims;
using MediatR;
using ArbTech.SharedKernel.Extensions;

namespace ArbTech.Infrastructure.DataAccess;

public class Repository<T>(
    ArbTechDbContext dbContext,
    IMediator mediator,
    IClaimsPrincipalAdaptor claimsPrincipalAdaptor)
    : ReadRepository<T>(dbContext), IRepository<T>
    where T : class, IBaseEntity, IAggregateRoot
{
    private readonly ArbTechDbContext _dbContext = dbContext;
    protected IMediator Mediator => mediator;

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Add(entity);

        await SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        // ReSharper disable once PossibleMultipleEnumeration
        _dbContext.Set<T>().AddRange(entities);

        await SaveChangesAsync(cancellationToken);

        // ReSharper disable once PossibleMultipleEnumeration
        return entities;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);

        await SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().UpdateRange(entities);

        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);

        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().RemoveRange(entities);

        await SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await InnerBeforeSaveActions(cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken);
    }


    protected virtual Task InnerBeforeSaveActions(CancellationToken cancellationToken)
    {
        DateTime timeStamp = DateTime.UtcNow;

        ClaimsPrincipal? claim = claimsPrincipalAdaptor.GetUserClaims();

        SignEntities(claim, timeStamp);

        return PublishDomainEvents(timeStamp, claim, cancellationToken);
    }


    private async Task PublishDomainEvents(DateTime timeStamp, ClaimsPrincipal? claims, CancellationToken cancellationToken)
    {
        // dispatch events  
        List<BaseDomainEvent> events = _dbContext.ChangeTracker.Entries<IHaveDomainEvents>()
            .Select(e => e.Entity)
            .SelectMany(i => i.GetDomainEvents())
            .ToList();

        // ignore events if no dispatcher provided
        if (events.Count == 0) return;
        foreach (BaseDomainEvent domainEvent in events)
        {
            domainEvent.DateOccurred = timeStamp;
            domainEvent.Claims = claims;
            await mediator.Publish(domainEvent, cancellationToken);
        }

        // reset events
        foreach (var entity in
                 _dbContext.ChangeTracker.Entries<IHaveDomainEvents>().Select(i => i.Entity))
        {
            entity.ClearDomainEvents();
        }
        
        // recall sign entities for domain handler entities
        SignEntities(claims, timeStamp);
    }

    private void SignEntities(ClaimsPrincipal? claimsPrincipal, DateTime timeStamp)
    {
        if (claimsPrincipal?.GetPartyId() is null)
            throw new AuthenticationException("'party_id' claim not found");

        List<object> addedEntries = _dbContext.ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified)
            .Select(i => i.Entity).ToList();
        foreach (object entry in addedEntries)
            if (entry is ISignEntity b)
                b.Sign(claimsPrincipal, timeStamp);
    }
}
