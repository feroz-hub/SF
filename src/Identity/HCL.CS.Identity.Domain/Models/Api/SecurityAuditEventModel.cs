/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Api;

public sealed class SecurityAuditEventModel
{
    public required string EventType { get; init; }
    public string? ActorUserId { get; init; }
    public string? PseudonymousIdentifier { get; init; }
    public string? AuthenticationSource { get; init; }
    public string? ClientId { get; init; }
    public string? GrantType { get; init; }
    public required string Result { get; init; }
    public string? ReasonCode { get; init; }
    public string? SessionId { get; init; }
}
