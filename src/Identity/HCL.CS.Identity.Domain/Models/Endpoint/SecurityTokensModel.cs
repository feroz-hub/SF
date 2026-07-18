/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Endpoint;

public class SecurityTokensModel : BaseModel
{
    public virtual string Key { get; set; }

    public virtual string TokenType { get; set; }

    public virtual string TokenValue { get; set; }

    public virtual DateTime? ConsumedTime { get; set; }

    public virtual DateTime? ConsumedAt { get; set; }

    public virtual bool TokenReuseDetected { get; set; }

    public virtual string ClientId { get; set; }

    public virtual string SessionId { get; set; }

    public virtual string UserId { get; set; }

    public virtual string SubjectId { get; set; }

    public virtual DateTime CreationTime { get; set; }

    public virtual int ExpiresAt { get; set; }
}
