using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using HCL.CS.Domain.Enums;
using HCL.CS.Infrastructure.Resources;
using Xunit;

namespace HCL.CS.UnitTests;

public class SigningCertificateLoaderTests
{
    private const string PublishedTestPassword = "hcl-cs-test-only-password";

    [Fact]
    public void PasswordProtectedRsaPfx_LoadsWithPrivateKeyAndConfiguredKeyId()
    {
        using var certificate = CreateRsaCertificate(
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddDays(1));
        var pfx = certificate.Export(X509ContentType.Pfx, PublishedTestPassword);

        using var loaded = SigningCertificateLoader.LoadFromBytes(
            pfx,
            PublishedTestPassword,
            SigningAlgorithm.RS256,
            "test-rsa-key").Certificate;

        loaded.HasPrivateKey.Should().BeTrue();
    }

    [Fact]
    public void PasswordProtectedPfx_LoadsFromTemporaryFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"hcl-cs-certificate-test-{Guid.NewGuid():N}");
        var path = Path.Combine(directory, "signing.pfx");
        Directory.CreateDirectory(directory);
        try
        {
            using var certificate = CreateRsaCertificate(
                DateTimeOffset.UtcNow.AddMinutes(-5),
                DateTimeOffset.UtcNow.AddDays(1));
            File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, PublishedTestPassword));

            using var loaded = SigningCertificateLoader.LoadFromFile(
                path,
                PublishedTestPassword,
                SigningAlgorithm.RS256,
                "test-file-key").Certificate;

            loaded.HasPrivateKey.Should().BeTrue();
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
            if (Directory.Exists(directory)) Directory.Delete(directory);
        }
    }

    [Fact]
    public void MissingPfx_IsRejected()
    {
        var missingPath = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.pfx");

        Action load = () => SigningCertificateLoader.LoadFromFile(
            missingPath,
            PublishedTestPassword,
            SigningAlgorithm.RS256,
            "missing-key");

        load.Should().Throw<InvalidOperationException>()
            .WithMessage("*does not exist*");
    }

    [Fact]
    public void WrongPassword_IsRejectedWithoutEchoingPassword()
    {
        using var certificate = CreateRsaCertificate(
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddDays(1));
        var pfx = certificate.Export(X509ContentType.Pfx, PublishedTestPassword);

        Action load = () => SigningCertificateLoader.LoadFromBytes(
            pfx,
            "incorrect-password",
            SigningAlgorithm.RS256,
            "wrong-password-key");

        load.Should().Throw<InvalidOperationException>()
            .Which.Message.Should().NotContain("incorrect-password");
    }

    [Fact]
    public void CertificateWithoutPrivateKey_IsRejected()
    {
        using var certificate = CreateRsaCertificate(
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddDays(1));
        var publicCertificate = certificate.Export(X509ContentType.Cert);

        Action load = () => SigningCertificateLoader.LoadFromBytes(
            publicCertificate,
            PublishedTestPassword,
            SigningAlgorithm.RS256,
            "public-only-key");

        load.Should().Throw<InvalidOperationException>()
            .WithMessage("*private key*");
    }

    [Fact]
    public void ExpiredCertificate_IsRejected()
    {
        using var certificate = CreateRsaCertificate(
            DateTimeOffset.UtcNow.AddDays(-2),
            DateTimeOffset.UtcNow.AddDays(-1));
        var pfx = certificate.Export(X509ContentType.Pfx, PublishedTestPassword);

        Action load = () => SigningCertificateLoader.LoadFromBytes(
            pfx,
            PublishedTestPassword,
            SigningAlgorithm.RS256,
            "expired-key");

        load.Should().Throw<InvalidOperationException>()
            .WithMessage("*expired*");
    }

    [Fact]
    public void NotYetValidCertificate_IsRejected()
    {
        using var certificate = CreateRsaCertificate(
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2));
        var pfx = certificate.Export(X509ContentType.Pfx, PublishedTestPassword);

        Action load = () => SigningCertificateLoader.LoadFromBytes(
            pfx,
            PublishedTestPassword,
            SigningAlgorithm.RS256,
            "future-key");

        load.Should().Throw<InvalidOperationException>()
            .WithMessage("*not yet valid*");
    }

    [Fact]
    public void AlgorithmMismatch_IsRejected()
    {
        using var certificate = CreateEcdsaCertificate(
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddDays(1));
        var pfx = certificate.Export(X509ContentType.Pfx, PublishedTestPassword);

        Action load = () => SigningCertificateLoader.LoadFromBytes(
            pfx,
            PublishedTestPassword,
            SigningAlgorithm.RS256,
            "algorithm-mismatch-key");

        load.Should().Throw<InvalidOperationException>()
            .WithMessage("*RSA private key*");
    }

    [Fact]
    public void InvalidKeyId_IsRejected()
    {
        using var certificate = CreateRsaCertificate(
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddDays(1));
        var pfx = certificate.Export(X509ContentType.Pfx, PublishedTestPassword);

        Action load = () => SigningCertificateLoader.LoadFromBytes(
            pfx,
            PublishedTestPassword,
            SigningAlgorithm.RS256,
            "invalid key id");

        load.Should().Throw<InvalidOperationException>()
            .WithMessage("*key ID*");
    }

    private static X509Certificate2 CreateRsaCertificate(
        DateTimeOffset notBefore,
        DateTimeOffset notAfter)
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            $"CN=HCL.CS Test RSA {Guid.NewGuid():N}",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        AddSigningExtensions(request);
        return request.CreateSelfSigned(notBefore, notAfter);
    }

    private static X509Certificate2 CreateEcdsaCertificate(
        DateTimeOffset notBefore,
        DateTimeOffset notAfter)
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var request = new CertificateRequest(
            $"CN=HCL.CS Test ECDSA {Guid.NewGuid():N}",
            ecdsa,
            HashAlgorithmName.SHA256);
        AddSigningExtensions(request);
        return request.CreateSelfSigned(notBefore, notAfter);
    }

    private static void AddSigningExtensions(CertificateRequest request)
    {
        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, true));
        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
        request.CertificateExtensions.Add(
            new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
    }
}
