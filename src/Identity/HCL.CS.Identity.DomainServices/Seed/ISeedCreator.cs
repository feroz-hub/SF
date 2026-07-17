using HCL.CS.Domain;

namespace HCL.CS.DomainServices.Seed;

public interface ISeedCreator
{
    Task<FrameworkResult> CreateMasterDataAsync();
    Task<bool> AddSeedModelsAsync(int orderNumber, BaseModel model);
}
