using System.Security.Claims;

namespace ArbTech.SharedKernel.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetPartyId(this ClaimsPrincipal principal)
        => int.TryParse(principal.FindFirst("party_id")?.Value, out var partyId) ? partyId: null;
}
