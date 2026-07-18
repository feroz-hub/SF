/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Endpoint;

public class LogoutMessageModel
{
    public string ClientId { get; set; }

    public string PostLogoutRedirectUri { get; set; }

    public string SubjectId { get; set; }

    public string SessionId { get; set; }

    public IEnumerable<string> ClientIdCollection { get; set; }

    public Dictionary<string, string> Parameters { get; set; } = new();

    public bool HasClient => !string.IsNullOrWhiteSpace(ClientId) || ClientIdCollection?.Any() == true;
}
