using Zentra.Domain.Models.Endpoint;

namespace Zentra.DomainServices.Infra;

public interface IKeyStore
{
    Dictionary<string, AsymmetricKeyInfoModel> Add(IEnumerable<AsymmetricKeyInfoModel> securityKeys);
}
