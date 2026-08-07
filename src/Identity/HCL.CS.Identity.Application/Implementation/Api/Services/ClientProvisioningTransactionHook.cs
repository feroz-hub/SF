using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public sealed class ClientProvisioningTransactionHook : IClientProvisioningTransactionHook
{
    public Task BeforeCommitAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
