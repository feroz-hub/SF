using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using HclCsInstallerMVC.Application.Abstractions;
using HclCsInstallerMVC.Application.DTOs;

namespace HclCsInstallerMVC.Infrastructure.Persistence;

public sealed class ProtectedInstallerStateStore : IInstallerStateStore
{
    private const string SessionKey = "hcl-cs-installer-state";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IDataProtector _protector;

    public ProtectedInstallerStateStore(IHttpContextAccessor httpContextAccessor,
        IDataProtectionProvider dataProtectionProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _protector = dataProtectionProvider.CreateProtector("HclCsInstallerMVC.SessionState.v1");
        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public async Task<InstallerSessionState> GetAsync(CancellationToken cancellationToken)
    {
        var session = GetSession();
        await session.LoadAsync(cancellationToken);

        var protectedPayload = session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(protectedPayload)) return new InstallerSessionState();

        try
        {
            var json = _protector.Unprotect(protectedPayload);
            return JsonSerializer.Deserialize<InstallerSessionState>(json, _jsonSerializerOptions) ??
                   new InstallerSessionState();
        }
        catch (CryptographicException)
        {
            await ClearAsync(cancellationToken);
            return new InstallerSessionState();
        }
    }

    public async Task SaveAsync(InstallerSessionState state, CancellationToken cancellationToken)
    {
        var session = GetSession();
        await session.LoadAsync(cancellationToken);

        var json = JsonSerializer.Serialize(state, _jsonSerializerOptions);
        var protectedPayload = _protector.Protect(json);

        session.SetString(SessionKey, protectedPayload);
        await session.CommitAsync(cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken)
    {
        var session = GetSession();
        await session.LoadAsync(cancellationToken);
        session.Remove(SessionKey);
        await session.CommitAsync(cancellationToken);
    }

    private ISession GetSession()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
            throw new InvalidOperationException("No active HTTP context is available for installer state storage.");

        return context.Session;
    }
}
