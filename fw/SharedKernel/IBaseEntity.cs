using System.Security.Claims;

namespace ArbTech.SharedKernel;

public interface IBaseEntity
{
   // int Id { get; }
}

public interface ISignEntity
{
    void Sign(ClaimsPrincipal? claimsPrincipal, DateTime? utcNow);
}

public interface IHaveDomainEvents
{
    IEnumerable<BaseDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}
