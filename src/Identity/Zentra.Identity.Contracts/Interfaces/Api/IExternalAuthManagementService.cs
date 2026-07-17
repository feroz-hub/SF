using Zentra.Domain;
using Zentra.Domain.Models.Api;

namespace Zentra.Service.Interfaces.Interfaces.Api;

public interface IExternalAuthManagementService
{
    Task<List<ExternalAuthProviderConfigModel>> GetAllProvidersAsync();

    Task<ExternalAuthProviderConfigModel> GetProviderAsync(Guid id);

    Task<FrameworkResult> SaveProviderAsync(SaveExternalAuthProviderRequest request);

    Task<FrameworkResult> DeleteProviderAsync(DeleteExternalAuthProviderRequest request);

    Task<FrameworkResult> TestProviderAsync(TestExternalAuthProviderRequest request);

    Task<ExternalAuthFieldDefinitionsResponse> GetFieldDefinitionsAsync();
}
