using Zentra.DemoClientMvc.Services;

namespace Zentra.DemoClientMvc.Interface;

public interface IEndpointFlowService
{
    Task<EndpointTokenFlowResult> ExecuteResourceOwnerPasswordFlowAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default);

    Task<EndpointTokenFlowResult> ExecuteClientCredentialsFlowAsync(CancellationToken cancellationToken = default);
}
