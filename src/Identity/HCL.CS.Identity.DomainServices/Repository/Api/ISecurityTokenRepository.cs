/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Threading;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.DomainServices.Repository.Api;

public interface ISecurityTokenRepository
{
    Task<IList<TokenModel>> GetSecurityTokenAsync(PagingModel page,
        SecurityTokenOption option,
        DateTime? fromdate = null,
        DateTime? todate = null,
        IList<string> clientIds = null,
        IList<string> userIds = null,
        CancellationToken cancellationToken = default);
}
