using System.Security.Authentication;
using System.Security.Claims;
using ArbTech.SharedKernel.Extensions;

namespace ArbTech.SharedKernel;
 
public abstract class BaseEntity : IBaseEntity, ISignEntity, IHaveDomainEvents
{
    public int CreatedBy { get; private set; }
    public DateTime Created { get; protected set; }
    public int? UpdatedBy { get; protected set; }
    public DateTime? Updated { get; protected set; }
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Id { get; set; }

    public virtual void Sign(ClaimsPrincipal? claimsPrincipal, DateTime? utcNow)
    {
        int? userId = claimsPrincipal?.GetPartyId();
        if (userId is null)
            throw new AuthenticationException("'party_id' claim not found");
        
        switch (CreatedBy)
        {
            case default(int) when userId is > 0:
                CreatedBy = userId.Value;
                break;
            case > default(int) when UpdatedBy == default && userId is > 0:
                UpdatedBy = userId.Value;
                break;
        }

        if (Created == default)
        {
            Created = utcNow ?? DateTime.UtcNow;  // note postgresql expected time zone in date time fields 
        }
        else if (Updated == default)
        {
            UpdatedBy = userId;
            Updated ??= utcNow ?? DateTime.UtcNow;
        }
    }

    
    private readonly  List<BaseDomainEvent> _domainEvents = new();

    public IEnumerable<BaseDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
    protected void AddDomainEvent(BaseDomainEvent @event) => _domainEvents.Add(@event);
 
}
