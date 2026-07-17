using System.Security.Cryptography.X509Certificates;
using HCL.CS.Domain.Enums;

namespace HCL.CS.Domain.Models.Endpoint;

public class AsymmetricKeyInfoModel
{
    public string KeyId { get; set; }

    public SigningAlgorithm Algorithm { get; set; }

    public X509Certificate2 Certificate { get; set; }
}
