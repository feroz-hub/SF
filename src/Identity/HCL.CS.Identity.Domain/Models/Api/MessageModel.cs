/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Models.Api;

public class EmailMessageModel
{
    public Guid UserId { get; set; }

    public string FromAddress { get; set; }

    public string FromName { get; set; }

    public string ToAddress { get; set; }

    public string ToName { get; set; }

    public string Subject { get; set; }

    public string Content { get; set; }

    public string Activity { get; set; }

    public string TemplateName { get; set; }

    public string CC { get; set; }

    public Dictionary<string, string> Parameters { get; set; }
}

public class SMSMessage
{
    public Guid UserId { get; set; }

    public string To { get; set; }

    public string Content { get; set; }

    public string Activity { get; set; }

    public string TemplateName { get; set; }

    public Dictionary<string, string> Parameters { get; set; }
}
