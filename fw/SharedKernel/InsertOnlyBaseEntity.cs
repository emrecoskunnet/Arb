// ReSharper disable MemberCanBePrivate.Global

using System.Security.Authentication;
using System.Security.Claims;
using ArbTech.SharedKernel.Extensions;

namespace ArbTech.SharedKernel;

public abstract class InsertOnlyBaseEntity : IBaseEntity, ISignEntity, IHaveDomainEvents
{
    public int CreatedBy { get; protected set; }
    public DateTime FromDate { get; protected set; }
    public DateTime? ThruDate { get; protected set; }

    public virtual int Id { get; set; }

    public virtual void Sign(ClaimsPrincipal? claimsPrincipal, DateTime? utcNow)
    {
        int? userId = claimsPrincipal?.GetPartyId();
        if (userId is null)
            throw new AuthenticationException("'party_id' claim not found");

        if (CreatedBy == default && userId > 0) CreatedBy = userId.Value;
        if (FromDate == default) FromDate = utcNow ?? DateTime.UtcNow;    //note postgresql expected time zone in date time fields
    }

    public virtual void SoftDelete(DateTime? utcNow = null)
    {
        if (FromDate == default) FromDate = utcNow ?? DateTime.UtcNow;
        ThruDate = utcNow ?? DateTime.UtcNow;
    }
    
    private readonly  List<BaseDomainEvent> _domainEvents = new();

    public IEnumerable<BaseDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
    protected void AddDomainEvent(BaseDomainEvent @event) => _domainEvents.Add(@event);
    
    
}
