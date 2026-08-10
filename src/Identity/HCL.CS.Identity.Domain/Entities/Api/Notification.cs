/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Entities.Api;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }

    public string MessageId { get; set; }

    public NotificationTypes Type { get; set; }

    public string Activity { get; set; }

    public NotificationStatus Status { get; set; }

    public string Sender { get; set; }

    public string Recipient { get; set; }

    public virtual Users User { get; set; }
}
