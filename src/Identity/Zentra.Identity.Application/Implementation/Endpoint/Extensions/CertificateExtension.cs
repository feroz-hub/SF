using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Math;
using Zentra.Domain.Models.Endpoint;

namespace Zentra.Service.Implementation.Endpoint.Extensions;

internal static class CertificateExtension
{
    // TODO: Need to move constants in this file below to resources.
    private const string InvalidCertificate = "Invalid certificate / No certificate found.";
    private const string UnsupportedAlgorithm = "Algorithm not supported.";
    private const string InvalidCertificateRsa = "Invalid Certificate - No RSA private key found.";
    private const string InvalidCertificatEcdsa = "Invalid Certificate - No ECDSA private key found.";
    private const string EmptySubject = "Subject name is empty in request.";
    private const string CertificateNotFound = "No certificate was found for subject name: ";
    private const string EcdsaCertificatePathInvalid = "ECDSA certificate path is empty.";
    private const string EcdsaCertificatePasswordInvalid = "ECDSA certificate password is empty.";
    private const string EcdsaCertificateEmpty = "ECDsa certificate is empty.";
    private const string EcdsaPublicKeyEmpty = "ECDsa public key is empty.";
    private const string JwtHeaderKeyValueEmpty = "Key value to generate JWT header is empty.";

    internal static SigningCredentials GetSymmetricCredentials(this string clientSecret, string algorithm)
    {
        SigningCredentials credentials = null;
        if (!string.IsNullOrWhiteSpace(algorithm))
        {
            if (algorithm.StartsWith("HS"))
            {
                if (!string.IsNullOrWhiteSpace(clientSecret))
                {
                    var securityKey = Encoding.ASCII.GetBytes(clientSecret);
                    credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), algorithm);
                }
                else
                {
                    throw new ArgumentNullException(nameof(clientSecret), "No client secret found");
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(algorithm), "Invalid Algorithm");
            }
        }

        return credentials;
    }

    internal static string GetAsymmetricCertificateHash(this Dictionary<string, AsymmetricKeyInfoModel> keyStore,
        string algorithm)
    {
        var certificateHash = string.Empty;
        if (!string.IsNullOrWhiteSpace(algorithm))
        {
            if (keyStore.Values.Count > 0)
            {
                var certificate = keyStore[algorithm].Certificate;
                if (certificate != null)
                    certificateHash = certificate.Thumbprint;
                //certificateHash = certificate.GetCertHash().Encode();
                else
                    throw new ArgumentNullException(nameof(certificate), "Invalid certificate / No certificate found");
            }
            else
            {
                throw new ArgumentNullException(nameof(keyStore), "No keys found in keyStore");
            }
        }
        else
        {
            throw new ArgumentNullException(nameof(keyStore), "Algorithm not supported.");
        }

        return certificateHash;
    }

    internal static SigningCredentials GetAsymmetricSigningCredentials(
        this Dictionary<string, AsymmetricKeyInfoModel> keyStore, string algorithm)
    {
        SigningCredentials credentials = null;
        if (!string.IsNullOrWhiteSpace(algorithm))
        {
            if (keyStore.Values.Count > 0)
            {
                var certificate = keyStore[algorithm].Certificate;
                if (certificate == null) throw new InvalidOperationException(InvalidCertificate);

                credentials = certificate.GenerateAsymmetricSigningCredentials(algorithm);
            }
            else
            {
                throw
                    new InvalidOperationException(
                        UnsupportedAlgorithm); // TODO: Talk to team on correcting this message text.
            }
        }

        return credentials;
    }

    internal static SigningCredentials GetAsymmetricVerificationCredentials(
        this Dictionary<string, AsymmetricKeyInfoModel> keyStore, string algorithm)
    {
        SigningCredentials credentials = null;
        if (!string.IsNullOrWhiteSpace(algorithm))
        {
            if (keyStore.Values.Count > 0)
            {
                var certificate = keyStore[algorithm].Certificate;
                if (certificate == null) throw new InvalidOperationException(InvalidCertificate);

                credentials = certificate.GenerateAsymmetricVerificationCredentials(algorithm);
                if (credentials?.Key != null && !string.IsNullOrWhiteSpace(keyStore[algorithm].KeyId))
                    credentials.Key.KeyId = keyStore[algorithm].KeyId;
            }
            else
            {
                throw
                    new InvalidOperationException(
                        UnsupportedAlgorithm); // TODO: Talk to team on correcting this message text.
            }
        }

        return credentials;
    }

    internal static SigningCredentials GenerateAsymmetricSigningCredentials(this X509Certificate2 certificate,
        string algorithm)
    {
        SecurityKey securityKey = null;
        algorithm = algorithm.ToUpper();
        if (algorithm.StartsWith("RS") || algorithm.StartsWith("PS"))
        {
            if (certificate.HasPrivateKey)
            {
                var rsa = certificate.GetRSAPrivateKey();
                if (rsa == null) throw new InvalidOperationException(InvalidCertificateRsa);

                securityKey = new RsaSecurityKey(rsa);
            }
        }
        else if (algorithm.StartsWith("ES"))
        {
            if (certificate.HasPrivateKey)
            {
                var ecdsa = certificate.GetECDsaPrivateKey();
                if (ecdsa == null) throw new InvalidOperationException(InvalidCertificatEcdsa);

                securityKey = new ECDsaSecurityKey(ecdsa);
            }
        }
        else
        {
            throw new InvalidOperationException(UnsupportedAlgorithm);
        }

        var signingCredentials = new SigningCredentials(securityKey, algorithm);
        return signingCredentials;
    }

    internal static SigningCredentials GenerateAsymmetricVerificationCredentials(this X509Certificate2 certificate,
        string algorithm)
    {
        SecurityKey securityKey = null;
        algorithm = algorithm.ToUpper();
        if (algorithm.StartsWith("RS") || algorithm.StartsWith("PS"))
        {
            if (certificate != null)
            {
                var rsa = certificate.GetRSAPublicKey();
                if (rsa == null) throw new InvalidOperationException(InvalidCertificateRsa);

                securityKey = new X509SecurityKey(certificate);
            }
        }
        else if (algorithm.StartsWith("ES"))
        {
            if (certificate != null)
            {
                var ecdsa = certificate.GetECDsaPublicKey();
                if (ecdsa == null) throw new InvalidOperationException(InvalidCertificatEcdsa);

                securityKey = new X509SecurityKey(certificate);
            }
        }
        else
        {
            throw new InvalidOperationException(UnsupportedAlgorithm);
        }

        var signingCredentials = new SigningCredentials(securityKey, algorithm);
        return signingCredentials;
    }

    internal static X509Certificate2 GetCertificateFromStore(StoreName name, StoreLocation location, string subjectName)
    {
        if (string.IsNullOrWhiteSpace(subjectName)) throw new InvalidOperationException(EmptySubject);

        var store = new X509Store(name, location);
        X509Certificate2Collection certificates = null;
        store.Open(OpenFlags.ReadOnly);
        try
        {
            X509Certificate2 result = null;
            certificates = store.Certificates;
            for (var count = 0; count < certificates.Count; count++)
            {
                var cert = certificates[count];
                if (cert.SubjectName.Name != null && cert.SubjectName.Name.ToLower().Contains(subjectName.ToLower()))
                {
                    result = new X509Certificate2(cert);
                    break;
                }
            }

            if (result == null) throw new InvalidOperationException(CertificateNotFound + subjectName);

            return result;
        }
        finally
        {
            if (certificates != null)
                foreach (var cert in certificates)
                    cert.Reset();

            store.Close();
            store.Dispose();
        }
    }

    internal static SecurityKey GetEcdsaPublicSigningKey(string ecdsaCertificatePath, string password)
    {
        if (string.IsNullOrWhiteSpace(ecdsaCertificatePath))
            throw new InvalidOperationException(EcdsaCertificatePathInvalid);

        if (string.IsNullOrWhiteSpace(password)) throw new InvalidOperationException(EcdsaCertificatePasswordInvalid);

        var certificate = new X509Certificate2(ecdsaCertificatePath, password,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
        return GetEcdsaPublicSigningKey(certificate);
    }

    internal static SecurityKey GetEcdsaPublicSigningKey(X509Certificate2 certificate)
    {
        if (certificate == null) throw new InvalidOperationException(EcdsaCertificateEmpty);

        var publicKey = certificate.GetECDsaPublicKey();
        SecurityKey rsaKey = new ECDsaSecurityKey(publicKey);
        return rsaKey;
    }

    internal static SecurityKey GetEcdsaPublicSigningKey(byte[] publicKey)
    {
        if (publicKey.Length == 0) throw new InvalidOperationException(EcdsaPublicKeyEmpty);

        var ecdsa = LoadEcdsaPublicKey(publicKey);
        var rsaKey = new ECDsaSecurityKey(ecdsa);
        return rsaKey;
    }

    internal static byte[] FromHexString(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) throw new InvalidOperationException(JwtHeaderKeyValueEmpty);

        var numberChars = hex.Length;
        var hexAsBytes = new byte[numberChars / 2];
        for (var count = 0; count < numberChars; count += 2)
            hexAsBytes[count / 2] = Convert.ToByte(hex.Substring(count, 2), 16);

        return hexAsBytes;
    }

    internal static ECDsa LoadEcdsaPublicKey(byte[] key)
    {
        var pubKeyX = key.Skip(1).Take(32).ToArray();
        var pubKeyY = key.Skip(33).ToArray();

        return ECDsa.Create(new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            Q = new ECPoint
            {
                X = pubKeyX,
                Y = pubKeyY
            }
        });
    }

    internal static ECDsa LoadEcdsaPrivateKey(byte[] key)
    {
        var mathKey = new BigInteger(+1, key);
        var parameters = SecNamedCurves.GetByName("secp256r1"); // TODO: Needs to be corrected.
        var ecPoint = parameters.G.Multiply(mathKey);
        var keyX = ecPoint.Normalize().XCoord.ToBigInteger().ToByteArrayUnsigned();
        var keyY = ecPoint.Normalize().YCoord.ToBigInteger().ToByteArrayUnsigned();

        return ECDsa.Create(new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            D = mathKey.ToByteArrayUnsigned(),
            Q = new ECPoint
            {
                X = keyX,
                Y = keyY
            }
        });
    }
}
