using HCL.CS.DemoClientMvc.Services;

namespace HCL.CS.DemoClientMvc.Interface;

public interface IEndpointFlowService
{
    Task<EndpointTokenFlowResult> ExecuteResourceOwnerPasswordFlowAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default);

    Task<EndpointTokenFlowResult> ExecuteClientCredentialsFlowAsync(CancellationToken cancellationToken = default);
}
