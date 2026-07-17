using System.Security.Cryptography.X509Certificates;
using Zentra.Domain.Enums;

namespace Zentra.Domain.Models.Endpoint;

public class AsymmetricKeyInfoModel
{
    public string KeyId { get; set; }

    public SigningAlgorithm Algorithm { get; set; }

    public X509Certificate2 Certificate { get; set; }
}
