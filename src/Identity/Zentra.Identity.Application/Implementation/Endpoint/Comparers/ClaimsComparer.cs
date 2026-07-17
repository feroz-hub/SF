using System.Security.Claims;

namespace Zentra.Service.Implementation.Endpoint.Comparers;

internal class ClaimsComparer : IEqualityComparer<Claim>
{
    public bool Equals(Claim sourceClaim, Claim targetClaim)
    {
        if (ReferenceEquals(sourceClaim, targetClaim)) return true;

        if (ReferenceEquals(sourceClaim, null) || ReferenceEquals(targetClaim, null)) return false;

        return sourceClaim.Type.ToLower() == targetClaim.Type.ToLower() &&
               sourceClaim.Value.ToLower() == targetClaim.Value.ToLower();
    }

    public int GetHashCode(Claim claim)
    {
        if (ReferenceEquals(claim, null)) return 0;

        var hashClaimType = claim.Type == null ? 0 : claim.Type.GetHashCode();
        var hashClaimValue = claim.Value.GetHashCode();
        return hashClaimType ^ hashClaimValue;
    }
}
