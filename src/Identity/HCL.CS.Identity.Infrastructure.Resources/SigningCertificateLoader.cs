/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
 */

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using HCL.CS.Domain.Enums;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.Infrastructure.Resources;

/// <summary>
/// Loads password-protected persistent signing certificates without exporting private-key material.
/// macOS does not support EphemeralKeySet for PFX imports, so it uses the platform default key store.
/// Linux and Windows use process-lifetime key storage while the PFX remains the persistent source.
/// </summary>
public static partial class SigningCertificateLoader
{
    public static AsymmetricKeyInfoModel LoadFromFile(
        string certificatePath,
        string password,
        SigningAlgorithm algorithm,
        string keyId,
        DateTimeOffset? utcNow = null)
    {
        if (string.IsNullOrWhiteSpace(certificatePath))
            throw new InvalidOperationException("A signing certificate path is required.");
        if (!File.Exists(certificatePath))
            throw new InvalidOperationException("The configured signing certificate file does not exist.");

        return Load(
            () => new X509Certificate2(certificatePath, RequirePassword(password), GetStorageFlags()),
            algorithm,
            keyId,
            utcNow);
    }

    public static AsymmetricKeyInfoModel LoadFromBytes(
        ReadOnlySpan<byte> certificateBytes,
        string password,
        SigningAlgorithm algorithm,
        string keyId,
        DateTimeOffset? utcNow = null)
    {
        if (certificateBytes.IsEmpty)
            throw new InvalidOperationException("Signing certificate data is empty.");

        var bytes = certificateBytes.ToArray();
        try
        {
            return Load(
                () => new X509Certificate2(bytes, RequirePassword(password), GetStorageFlags()),
                algorithm,
                keyId,
                utcNow);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(bytes);
        }
    }

    private static AsymmetricKeyInfoModel Load(
        Func<X509Certificate2> certificateFactory,
        SigningAlgorithm algorithm,
        string keyId,
        DateTimeOffset? utcNow)
    {
        ValidateKeyId(keyId);

        X509Certificate2 certificate;
        try
        {
            certificate = certificateFactory();
        }
        catch (CryptographicException exception)
        {
            throw new InvalidOperationException(
                "The signing certificate could not be loaded. Verify the PFX and its password.",
                exception);
        }

        try
        {
            ValidateCertificate(certificate, algorithm, utcNow ?? DateTimeOffset.UtcNow);
            return new AsymmetricKeyInfoModel
            {
                Certificate = certificate,
                Algorithm = algorithm,
                KeyId = keyId
            };
        }
        catch
        {
            certificate.Dispose();
            throw;
        }
    }

    private static X509KeyStorageFlags GetStorageFlags()
    {
        return OperatingSystem.IsMacOS()
            ? X509KeyStorageFlags.DefaultKeySet
            : X509KeyStorageFlags.EphemeralKeySet;
    }

    private static string RequirePassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password)
            ? password
            : throw new InvalidOperationException("A signing certificate password is required.");
    }

    private static void ValidateKeyId(string keyId)
    {
        if (string.IsNullOrWhiteSpace(keyId)
            || keyId.Length > 128
            || !KeyIdPattern().IsMatch(keyId))
        {
            throw new InvalidOperationException(
                "The signing certificate key ID must contain 1-128 letters, digits, dots, underscores, or hyphens.");
        }
    }

    private static void ValidateCertificate(
        X509Certificate2 certificate,
        SigningAlgorithm algorithm,
        DateTimeOffset utcNow)
    {
        if (!certificate.HasPrivateKey)
            throw new InvalidOperationException("The signing certificate does not contain a private key.");
        if (certificate.NotBefore.ToUniversalTime() > utcNow.UtcDateTime)
            throw new InvalidOperationException("The signing certificate is not yet valid.");
        if (certificate.NotAfter.ToUniversalTime() <= utcNow.UtcDateTime)
            throw new InvalidOperationException("The signing certificate has expired.");

        var keyUsage = certificate.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
        if (keyUsage is not null
            && (keyUsage.KeyUsages & X509KeyUsageFlags.DigitalSignature) == 0)
        {
            throw new InvalidOperationException(
                "The signing certificate is not permitted for digital signatures.");
        }

        var algorithmName = Enum.GetName(algorithm)
                            ?? throw new InvalidOperationException("The signing algorithm is unsupported.");
        if (algorithmName.StartsWith("RS", StringComparison.OrdinalIgnoreCase)
            || algorithmName.StartsWith("PS", StringComparison.OrdinalIgnoreCase))
        {
            using var rsa = certificate.GetRSAPrivateKey();
            if (rsa is null || rsa.KeySize < 2048)
                throw new InvalidOperationException(
                    "The signing certificate must contain an RSA private key of at least 2048 bits.");
            return;
        }

        if (algorithmName.StartsWith("ES", StringComparison.OrdinalIgnoreCase))
        {
            using var ecdsa = certificate.GetECDsaPrivateKey();
            if (ecdsa is null || ecdsa.KeySize < 256)
                throw new InvalidOperationException(
                    "The signing certificate must contain an ECDSA private key of at least 256 bits.");
            return;
        }

        throw new InvalidOperationException("The signing algorithm is unsupported.");
    }

    [GeneratedRegex("^[A-Za-z0-9._-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex KeyIdPattern();
}
