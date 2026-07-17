using Microsoft.AspNetCore.Identity;
using HCL.CS.Domain;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace HCL.CS.Service.Implementation.Api.Extension;

internal static class IdentityResultExtension
{
    internal static IEnumerable<FrameworkError> ConstructIdentityErrorAsList(this IdentityResult identityResult)
    {
        if (identityResult.Errors.ContainsAny())
            return identityResult.Errors.Select(x => new FrameworkError { Code = x.Code, Description = x.Description })
                .ToList();

        return Enumerable.Empty<FrameworkError>();
    }

    internal static string ConstructIdentityErrorAsString(this IdentityResult identityResult)
    {
        if (identityResult.Errors.ContainsAny())
            return string.Join(", ", identityResult.Errors.Select(e => e.Description));

        return null;
    }
}
