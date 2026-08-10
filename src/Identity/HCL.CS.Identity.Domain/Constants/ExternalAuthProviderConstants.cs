/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Domain.Constants;

public static class ExternalAuthProviderConstants
{
    public const string Google = "Google";

    public static readonly Dictionary<string, ProviderFieldDefinition[]> ProviderFields = new()
    {
        [Google] = new[]
        {
            new ProviderFieldDefinition("ClientId", "Client ID", "text", true),
            new ProviderFieldDefinition("ClientSecret", "Client Secret", "password", true),
            new ProviderFieldDefinition("Authority", "Authority", "text", false),
            new ProviderFieldDefinition("MetadataAddress", "Metadata Address", "text", false),
            new ProviderFieldDefinition("CallbackPath", "Callback Path", "text", false),
            new ProviderFieldDefinition("AllowedRedirectHosts", "Allowed Redirect Hosts", "textarea", false)
        }
    };

    public static readonly Dictionary<string, Dictionary<string, string>> ProviderDefaults = new()
    {
        [Google] = new Dictionary<string, string>
        {
            { "Authority", "https://accounts.google.com" },
            { "MetadataAddress", "https://accounts.google.com/.well-known/openid-configuration" },
            { "CallbackPath", "/auth/external/google/signin-callback" }
        }
    };
}
