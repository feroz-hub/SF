/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.DomainServices.Infra;

public interface IEmailProvider
{
    string ProviderName { get; }

    Task<ProviderSendResult> SendAsync(EmailMessage message, Dictionary<string, string> config);
}

public class EmailMessage
{
    public string From { get; set; }

    public string FromName { get; set; }

    public string To { get; set; }

    public string Subject { get; set; }

    public string HtmlBody { get; set; }

    public string CC { get; set; }
}

public class ProviderSendResult
{
    public bool Success { get; set; }

    public string MessageId { get; set; }

    public string ErrorMessage { get; set; }
}
