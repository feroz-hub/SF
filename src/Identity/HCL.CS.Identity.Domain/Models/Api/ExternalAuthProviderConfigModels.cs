/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.Domain.Constants;

namespace HCL.CS.Domain.Models.Api;

public class ExternalAuthProviderConfigModel
{
    public Guid? Id { get; set; }

    public string ProviderName { get; set; }

    public int ProviderType { get; set; }

    public bool IsEnabled { get; set; }

    public Dictionary<string, string> Settings { get; set; }

    public bool AutoProvisionEnabled { get; set; }

    public string AllowedDomains { get; set; }

    public DateTime? LastTestedOn { get; set; }

    public bool? LastTestSuccess { get; set; }
}

public class SaveExternalAuthProviderRequest
{
    public Guid? Id { get; set; }

    public string ProviderName { get; set; }

    public int ProviderType { get; set; }

    public bool IsEnabled { get; set; }

    public Dictionary<string, string> Settings { get; set; }

    public bool AutoProvisionEnabled { get; set; }

    public string AllowedDomains { get; set; }
}

public class DeleteExternalAuthProviderRequest
{
    public Guid Id { get; set; }
}

public class TestExternalAuthProviderRequest
{
    public Guid Id { get; set; }
}

public class ExternalAuthFieldDefinitionsResponse
{
    public Dictionary<string, ProviderFieldDefinition[]> Providers { get; set; }

    public Dictionary<string, Dictionary<string, string>> Defaults { get; set; }
}
