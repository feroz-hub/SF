using Zentra.DomainServices.Infra;

namespace Zentra.Infrastructure.Services.Implementation.Providers;

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
