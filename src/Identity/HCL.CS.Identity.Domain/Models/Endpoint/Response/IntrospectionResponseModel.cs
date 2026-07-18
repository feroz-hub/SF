/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.ComponentModel;

namespace HCL.CS.Domain.Models.Endpoint.Response;

public class IntrospectionResponseModel
{
    [DisplayName("active")] public bool Active { get; set; }

    [DisplayName("client_id")] public string ClientId { get; set; }

    [DisplayName("username")] public string UserName { get; set; }

    [DisplayName("scope")] public string Scope { get; set; }

    [DisplayName("sub")] public string SubjectId { get; set; }

    [DisplayName("aud")] public string Audience { get; set; }

    [DisplayName("iss")] public string Issuer { get; set; }

    [DisplayName("exp")] public string ExpiresAt { get; set; }

    [DisplayName("iat")] public string IssuedAt { get; set; }
}
