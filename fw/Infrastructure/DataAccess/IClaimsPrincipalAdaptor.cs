using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ArbTech.Infrastructure.DataAccess;

public interface IClaimsPrincipalAdaptor
{
    ClaimsPrincipal? GetUserClaims();

    void SetClaims(ClaimsPrincipal principal);
}

public class DefaultClaimsPrincipalAdaptor(IHttpContextAccessor contextAccessor) : IClaimsPrincipalAdaptor
{
    private readonly ConcurrentBag<ClaimsPrincipal> _concurrentClaims = new();
    public ClaimsPrincipal? GetUserClaims()
    {
        return contextAccessor.HttpContext?.User
            ?? _concurrentClaims.FirstOrDefault();
    }

    public void SetClaims(ClaimsPrincipal principal)
    {
        _concurrentClaims.Clear();
        _concurrentClaims.Add(principal);
    }
}
