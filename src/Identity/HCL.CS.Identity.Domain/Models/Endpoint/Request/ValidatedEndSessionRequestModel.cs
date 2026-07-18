/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Domain.Models.Endpoint.Request;

public class ValidatedEndSessionRequestModel : ValidatedBaseModel
{
    public SecurityKey Key { get; set; }

    public string SubjectId { get; set; }

    public JwtSecurityToken DecodedToken { get; set; }

    public long ExpiresAt { get; set; }

    public string PostLogOutUri { get; set; }

    public string State { get; set; }

    public IEnumerable<string> ClientIds { get; set; }
}
