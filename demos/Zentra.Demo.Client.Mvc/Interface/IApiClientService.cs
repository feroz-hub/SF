using Zentra.DemoClientMvc.Services;

namespace Zentra.DemoClientMvc.Interface;

public interface IApiClientService
{
    Task<ApiClientResponse> GetAsync(string relativePath, CancellationToken cancellationToken = default);

    Task<ApiClientResponse> PostAsync<TRequest>(string relativePath, TRequest payload,
        CancellationToken cancellationToken = default);
}
