/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
