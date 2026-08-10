namespace HCL.CS.Service.Interfaces.Interfaces.Api;

/// <summary>
/// Runs after provisioning changes are flushed but before their transaction is committed.
/// Implementations may enforce deployment-specific invariants; exceptions roll back the operation.
/// </summary>
public interface IClientProvisioningTransactionHook
{
    Task BeforeCommitAsync(string clientId, CancellationToken cancellationToken = default);
}
