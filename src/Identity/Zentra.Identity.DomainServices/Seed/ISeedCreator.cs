using Zentra.Domain;

namespace Zentra.DomainServices.Seed;

public interface ISeedCreator
{
    Task<FrameworkResult> CreateMasterDataAsync();
    Task<bool> AddSeedModelsAsync(int orderNumber, BaseModel model);
}
