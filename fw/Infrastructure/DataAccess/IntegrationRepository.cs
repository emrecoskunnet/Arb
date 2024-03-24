using System.Diagnostics.Tracing;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using ArbTech.Infrastructure.Messaging.Outbox.Services;
using ArbTech.SharedKernel.Messaging;

namespace ArbTech.Infrastructure.DataAccess;

public class IntegrationRepository<T>(
    ArbTechDbContext dbContext,
    IMediator mediator,
    IClaimsPrincipalAdaptor claimsPrincipalAdaptor,
    IIntegrationEventLogService integrationEventLogService,
    ILogger<IntegrationRepository<T>> logger)
    : Repository<T>(dbContext, mediator, claimsPrincipalAdaptor),
        IIntegrationRepository<T>
    where T : class, IBaseEntity, IAggregateRoot
{
    private readonly ArbTechDbContext _dbContext = dbContext;
    private readonly IClaimsPrincipalAdaptor _claimsPrincipalAdaptor = claimsPrincipalAdaptor;

    public async Task<T> AddAsync(T entity, Func<BaseIntegrationEvent?> eventBuilder,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Add(entity);

        await OutBoxIntegrationSaveChanges(eventBuilder, cancellationToken);

        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, Func<BaseIntegrationEvent?> eventBuilder,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<T> addRangeAsync = entities as T[] ?? entities.ToArray();
        _dbContext.Set<T>().AddRange(addRangeAsync);

        await OutBoxIntegrationSaveChanges(eventBuilder, cancellationToken);

        return addRangeAsync;
    }

    public async Task UpdateAsync(T entity, Func<BaseIntegrationEvent?> eventBuilder,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);

        await OutBoxIntegrationSaveChanges(eventBuilder, cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, Func<BaseIntegrationEvent?> eventBuilder,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().UpdateRange(entities);

        await OutBoxIntegrationSaveChanges(eventBuilder, cancellationToken);
    }

    public async Task DeleteAsync(T entity, Func<BaseIntegrationEvent?> eventBuilder,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);

        await OutBoxIntegrationSaveChanges(eventBuilder, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, Func<BaseIntegrationEvent?> eventBuilder,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().RemoveRange(entities);

        await OutBoxIntegrationSaveChanges(eventBuilder, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(Func<BaseIntegrationEvent?> eventBuilder,
        CancellationToken cancellationToken = default)
    {
        return await OutBoxIntegrationSaveChanges(eventBuilder, cancellationToken);
    }

    private async Task<int> OutBoxIntegrationSaveChanges(Func<BaseIntegrationEvent?> eventBuilder, CancellationToken cancellationToken)
    {
        Guard.Against.Null(eventBuilder);

        int result = -1;
        Guid transactionId = Guid.NewGuid();
        
        await InnerBeforeSaveActions(cancellationToken);
        //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
        //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
        await ResilientTransaction.New(_dbContext).ExecuteAsync(async () =>
        {
            transactionId = _dbContext.Database.CurrentTransaction?.TransactionId
                            ?? throw new InvalidOperationException("Invalid current transaction");

            logger.LogInformation("Begin transaction {TransactionId} )", transactionId);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var evt = eventBuilder();
            Guard.Against.Null(evt);
            MakeSureClaimsAdded(evt);
            
            await integrationEventLogService.SaveEventAsync(evt,
                _dbContext.Database.CurrentTransaction ??
                throw new InvalidOperationException("Invalid current transaction"));
            logger.LogInformation("Commit transaction {TransactionId}", transactionId);
        },
        async () =>
        {
            logger.LogInformation("Publish events through event bus {TransactionId} )", transactionId);
            await integrationEventLogService.PublishEventsThroughEventBusAsync(transactionId);
        });
 
 
        return result;

        void MakeSureClaimsAdded(BaseIntegrationEvent evt)
        {
            if (evt.Claims?.Count is > 0) return;
            var claims = _claimsPrincipalAdaptor.GetUserClaims();
            if (claims?.Claims.Count() is not > 0) return;
            evt.Claims = claims.Claims.Select(i => new ClaimInfo() { Type = i.Type, Value = i.Value }).ToArray();
        }
    }
}
 
