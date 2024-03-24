using System.Security.Claims;
using MediatR;

namespace ArbTech.SharedKernel;

public record BaseDomainEvent: INotification
{
    public DateTime DateOccurred { get; set; } = DateTime.UtcNow;
    public ClaimsPrincipal? Claims { get; set; }
}
