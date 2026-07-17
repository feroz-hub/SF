using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.DomainServices.Infra;

public interface IKeyStore
{
    Dictionary<string, AsymmetricKeyInfoModel> Add(IEnumerable<AsymmetricKeyInfoModel> securityKeys);
}
