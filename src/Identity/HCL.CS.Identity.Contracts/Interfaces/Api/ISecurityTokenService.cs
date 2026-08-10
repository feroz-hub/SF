/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface ISecurityTokenService
{
    Task<IList<TokenModel>> GetClientsActiveSecurityTokensAsync(IList<string> clientIds, PagingModel page = null);

    Task<IList<TokenModel>> GetUsersActiveSecurityTokensAsync(IList<string> userIds, PagingModel page = null);

    Task<IList<TokenModel>> GetActiveSecurityTokensAsync(DateTime fromdate, DateTime todate, PagingModel page = null);

    Task<IList<TokenModel>> GetAllSecurityTokensAsync(DateTime fromdate, DateTime todate, PagingModel page = null);
}
