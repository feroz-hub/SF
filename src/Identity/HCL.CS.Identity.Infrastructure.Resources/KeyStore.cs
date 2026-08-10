/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices.Infra;
using static HCL.CS.Domain.Constants.Endpoint.OpenIdConstants;

namespace HCL.CS.Infrastructure.Resources;

internal class KeyStore : IKeyStore
{
    public Dictionary<string, AsymmetricKeyInfoModel> Add(IEnumerable<AsymmetricKeyInfoModel> asymmetricKeyInfo)
    {
        var keyStore = new Dictionary<string, AsymmetricKeyInfoModel>();
        var validationMessage = string.Empty;
        var securityKeys = asymmetricKeyInfo.ToList();

        if (securityKeys.Any())
            foreach (var securityKey in securityKeys)
            {
                if (!securityKey.Certificate.HasPrivateKey)
                    validationMessage += securityKey.KeyId + Environment.NewLine + securityKey.Algorithm;

                validationMessage += CheckCertificateValidity(securityKey);

                if (string.IsNullOrWhiteSpace(validationMessage))
                {
                    var name = Enum.GetName(typeof(SigningAlgorithm), securityKey.Algorithm)?.ToUpper();

                    if (name != null && !keyStore.ContainsKey(name))
                        keyStore.Add(name, securityKey);
                    else
                        throw new Exception("Same Key/algorithm already added");
                }
            }

        if (!string.IsNullOrWhiteSpace(validationMessage))
            throw new Exception("Certificate does not have a private key :" + validationMessage);

        return keyStore;
    }

    public static bool AlgorithmExists(string algorithm)
    {
        var algorithmExists = false;
        var type = typeof(Algorithms);
        var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var fi in fields)
            if (fi.GetValue(null)?.ToString() == algorithm)
            {
                algorithmExists = true;
                break;
            }

        return algorithmExists;
    }

    private static string CheckCertificateValidity(AsymmetricKeyInfoModel securityKey)
    {
        var validationMessage = string.Empty;
        if (securityKey.Certificate.NotAfter < DateTime.UtcNow) validationMessage += "Certificate Expired";

        var name = Enum.GetName(typeof(SigningAlgorithm), securityKey.Algorithm)?.ToUpper();
        if (!AlgorithmExists(name)) validationMessage += "Algorithm type not supported";

        if (!VerifyCertificate(securityKey)) validationMessage += "Certificate and Algorithm type mismatch";

        return validationMessage;
    }

    private static bool VerifyCertificate(AsymmetricKeyInfoModel securityKey)
    {
        var result = false;
        var name = Enum.GetName(typeof(SigningAlgorithm), securityKey.Algorithm)?.ToUpper();
        if (name != null && (name.StartsWith("RS") || name.StartsWith("PS")))
        {
            using var privateKey = securityKey.Certificate.GetRSAPrivateKey();
            if (privateKey != null) result = true;
        }
        else if (name != null && name.StartsWith("ES"))
        {
            using var privateKey = securityKey.Certificate.GetECDsaPrivateKey();
            if (privateKey != null) result = true;
        }

        return result;
    }
}
