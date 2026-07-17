using HCL.CS.Domain;
using HCL.CS.Domain.Models.Api;

namespace HCL.CS.Service.Interfaces.Interfaces.Api;

public interface IExternalAuthManagementService
{
    Task<List<ExternalAuthProviderConfigModel>> GetAllProvidersAsync();

    Task<ExternalAuthProviderConfigModel> GetProviderAsync(Guid id);

    Task<FrameworkResult> SaveProviderAsync(SaveExternalAuthProviderRequest request);

    Task<FrameworkResult> DeleteProviderAsync(DeleteExternalAuthProviderRequest request);

    Task<FrameworkResult> TestProviderAsync(TestExternalAuthProviderRequest request);

    Task<ExternalAuthFieldDefinitionsResponse> GetFieldDefinitionsAsync();
}
