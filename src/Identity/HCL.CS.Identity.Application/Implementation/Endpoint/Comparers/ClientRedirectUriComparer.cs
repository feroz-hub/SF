/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Entities.Endpoint;

namespace HCL.CS.Service.Implementation.Endpoint.Comparers;

internal class ClientRedirectUriComparer : IEqualityComparer<ClientRedirectUris>
{
    public bool Equals(ClientRedirectUris x, ClientRedirectUris y)
    {
        if (x.ClientId == y.ClientId && x.RedirectUri.ToLower() == y.RedirectUri.ToLower()) return true;

        return false;
    }

    public int GetHashCode(ClientRedirectUris obj)
    {
        return obj.Id.GetHashCode();
    }
}
