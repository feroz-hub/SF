/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using HCL.CS.DomainServices.Infra;

namespace HCL.CS.Infrastructure.Services.Implementation.Providers;

public class NotificationProviderFactory
{
    private readonly Dictionary<string, IEmailProvider> _emailProviders;
    private readonly Dictionary<string, ISmsProvider> _smsProviders;

    public NotificationProviderFactory(
        IEnumerable<IEmailProvider> emailProviders,
        IEnumerable<ISmsProvider> smsProviders)
    {
        _emailProviders = emailProviders.ToDictionary(p => p.ProviderName, StringComparer.OrdinalIgnoreCase);
        _smsProviders = smsProviders.ToDictionary(p => p.ProviderName, StringComparer.OrdinalIgnoreCase);
    }

    public IEmailProvider GetEmailProvider(string providerName)
    {
        if (_emailProviders.TryGetValue(providerName, out var provider))
            return provider;

        throw new InvalidOperationException($"Email provider '{providerName}' is not registered.");
    }

    public ISmsProvider GetSmsProvider(string providerName)
    {
        if (_smsProviders.TryGetValue(providerName, out var provider))
            return provider;

        throw new InvalidOperationException($"SMS provider '{providerName}' is not registered.");
    }

    public IReadOnlyList<string> GetEmailProviderNames() => _emailProviders.Keys.ToList();

    public IReadOnlyList<string> GetSmsProviderNames() => _smsProviders.Keys.ToList();
}
