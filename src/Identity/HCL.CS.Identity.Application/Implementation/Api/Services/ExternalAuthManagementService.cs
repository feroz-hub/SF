using System.Text.Json;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.DomainServices;
using HCL.CS.DomainServices.Infra;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Services;

public class ExternalAuthManagementService : SecurityBase, IExternalAuthManagementService
{
    private readonly IRepository<ExternalAuthProviderConfig> _providerConfigRepository;
    private readonly IFrameworkResultService _frameworkResult;
    private readonly ILoggerService _loggerService;

    public ExternalAuthManagementService(
        IRepository<ExternalAuthProviderConfig> providerConfigRepository,
        IFrameworkResultService frameworkResult,
        ILoggerInstance loggerInstance)
    {
        _providerConfigRepository = providerConfigRepository;
        _frameworkResult = frameworkResult;
        _loggerService = loggerInstance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public virtual async Task<List<ExternalAuthProviderConfigModel>> GetAllProvidersAsync()
    {
        try
        {
            var configs = await _providerConfigRepository.GetAllAsync();
            return configs.Select(c => MapToModel(c, maskSecrets: true)).ToList();
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve external auth providers.");
            throw;
        }
    }

    public virtual async Task<ExternalAuthProviderConfigModel> GetProviderAsync(Guid id)
    {
        try
        {
            var config = await _providerConfigRepository.GetAsync(id);
            if (config == null)
                _frameworkResult.Throw(ApiErrorCodes.InvalidOrNullObject);

            return MapToModel(config, maskSecrets: true);
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to retrieve external auth provider.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> SaveProviderAsync(SaveExternalAuthProviderRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ProviderName))
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            if (request.Settings == null || request.Settings.Count == 0)
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            var configJson = JsonSerializer.Serialize(request.Settings);

            if (request.Id.HasValue && request.Id.Value != Guid.Empty)
            {
                var existing = await _providerConfigRepository.GetAsync(request.Id.Value);
                if (existing == null)
                    return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

                existing.ProviderName = request.ProviderName;
                existing.ProviderType = request.ProviderType;
                existing.IsEnabled = request.IsEnabled;
                existing.ConfigJson = configJson;
                existing.AutoProvisionEnabled = request.AutoProvisionEnabled;
                existing.AllowedDomains = request.AllowedDomains;
                await _providerConfigRepository.UpdateAsync(existing);
            }
            else
            {
                var newConfig = new ExternalAuthProviderConfig
                {
                    Id = Guid.NewGuid(),
                    ProviderName = request.ProviderName,
                    ProviderType = request.ProviderType,
                    IsEnabled = request.IsEnabled,
                    ConfigJson = configJson,
                    AutoProvisionEnabled = request.AutoProvisionEnabled,
                    AllowedDomains = request.AllowedDomains,
                    CreatedBy = "Admin"
                };

                await _providerConfigRepository.InsertAsync(newConfig);
            }

            return await _providerConfigRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to save external auth provider.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> DeleteProviderAsync(DeleteExternalAuthProviderRequest request)
    {
        try
        {
            var config = await _providerConfigRepository.GetAsync(request.Id);
            if (config == null)
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            if (config.IsEnabled)
                return _frameworkResult.ConstructFailed(ApiErrorCodes.InvalidOrNullObject,
                    "Cannot delete an enabled provider. Disable it first.");

            await _providerConfigRepository.DeleteAsync(config);
            return await _providerConfigRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to delete external auth provider.");
            throw;
        }
    }

    public virtual async Task<FrameworkResult> TestProviderAsync(TestExternalAuthProviderRequest request)
    {
        try
        {
            var config = await _providerConfigRepository.GetAsync(request.Id);
            if (config == null)
                return _frameworkResult.Failed<FrameworkResult>(ApiErrorCodes.InvalidOrNullObject);

            var settings = DeserializeSettings(config.ConfigJson);
            var metadataAddress = settings.GetValueOrDefault("MetadataAddress",
                "https://accounts.google.com/.well-known/openid-configuration");

            bool testSuccess;
            try
            {
                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var response = await httpClient.GetAsync(metadataAddress);
                testSuccess = response.IsSuccessStatusCode;
            }
            catch
            {
                testSuccess = false;
            }

            config.LastTestedOn = DateTime.UtcNow;
            config.LastTestSuccess = testSuccess;
            await _providerConfigRepository.UpdateAsync(config);
            await _providerConfigRepository.SaveChangesAsync();

            if (!testSuccess)
                return _frameworkResult.ConstructFailed(ApiErrorCodes.InvalidOrNullObject,
                    "Failed to reach provider metadata endpoint.");

            return _frameworkResult.Succeeded();
        }
        catch (Exception ex)
        {
            _loggerService.WriteToWithCaller(Log.Error, ex, "Failed to test external auth provider.");
            throw;
        }
    }

    public virtual Task<ExternalAuthFieldDefinitionsResponse> GetFieldDefinitionsAsync()
    {
        var response = new ExternalAuthFieldDefinitionsResponse
        {
            Providers = ExternalAuthProviderConstants.ProviderFields,
            Defaults = ExternalAuthProviderConstants.ProviderDefaults
        };

        return Task.FromResult(response);
    }

    private static ExternalAuthProviderConfigModel MapToModel(ExternalAuthProviderConfig config, bool maskSecrets)
    {
        var settings = DeserializeSettings(config.ConfigJson);

        if (maskSecrets && ExternalAuthProviderConstants.ProviderFields.TryGetValue(config.ProviderName, out var fields))
        {
            foreach (var field in fields)
            {
                if (field.InputType == "password" && settings.ContainsKey(field.Key))
                {
                    var value = settings[field.Key];
                    if (!string.IsNullOrEmpty(value) && value.Length > 4)
                        settings[field.Key] = value[..4] + "****";
                    else if (!string.IsNullOrEmpty(value))
                        settings[field.Key] = "****";
                }
            }
        }

        return new ExternalAuthProviderConfigModel
        {
            Id = config.Id,
            ProviderName = config.ProviderName,
            ProviderType = config.ProviderType,
            IsEnabled = config.IsEnabled,
            Settings = settings,
            AutoProvisionEnabled = config.AutoProvisionEnabled,
            AllowedDomains = config.AllowedDomains,
            LastTestedOn = config.LastTestedOn,
            LastTestSuccess = config.LastTestSuccess
        };
    }

    private static Dictionary<string, string> DeserializeSettings(string configJson)
    {
        if (string.IsNullOrWhiteSpace(configJson))
            return new Dictionary<string, string>();

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(configJson)
                ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
