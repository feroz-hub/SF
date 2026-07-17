using System.Security.Cryptography;
using System.Text;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Zentra.Domain.Constants.Endpoint;

namespace Zentra.Service.Implementation.Endpoint.Extensions;
// TODO: Need to move hardcoded data in this file below to resources. Currently FrameworkResultService is an interface but this file is an extension.

public static class EncryptionExtension
{
    private const int LegacyPbkdf2Iterations = 1000;
    private const string IllegalStringToDecode = "Illegal base64url string specified for decoding.";
    private const string RsaXmlEmpty = "Rsa XML content is empty.";
    private const string InvalidSigningAlgorithm = "Invalid signing algorithm: ";

    internal static string AesEncrypt(this string data, string key, int keySize)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var keyHashBytes = SHA256.Create().ComputeHash(keyBytes);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var saltBytes = keySize.RandomBytes();
        var encryptedBytes = new byte[saltBytes.Length + dataBytes.Length];

        // Combine Salt + Text
        for (var i = 0; i < saltBytes.Length; i++) encryptedBytes[i] = saltBytes[i];

        for (var i = 0; i < dataBytes.Length; i++) encryptedBytes[i + saltBytes.Length] = dataBytes[i];

        encryptedBytes = AesEncrypt(encryptedBytes, keyHashBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    internal static byte[] AesEncrypt(byte[] dataBytes, byte[] keyBytes)
    {
        var saltBytes = Encoding.ASCII.GetBytes("iavcpquyxdaxganojgytbfhitzpptqbn");
        using var ms = new MemoryStream();
        //using RijndaelManaged aes = new RijndaelManaged();
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;

        var key = new Rfc2898DeriveBytes(keyBytes, saltBytes, LegacyPbkdf2Iterations, HashAlgorithmName.SHA1);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);

        aes.Mode = CipherMode.CBC;
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(dataBytes, 0, dataBytes.Length);
            cs.Close();
        }

        return ms.ToArray();
    }

    internal static string AesDecrypt(this string data, string key, int keySize)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var keyHashBytes = SHA256.Create().ComputeHash(keyBytes);
        var dataBytes = Convert.FromBase64String(data);
        var decryptedbytes = AesDecrypt(dataBytes, keyHashBytes);

        var saltLength = keySize;
        var resultbytes = new byte[decryptedbytes.Length - saltLength];
        for (var i = 0; i < resultbytes.Length; i++) resultbytes[i] = decryptedbytes[i + saltLength];

        return Encoding.UTF8.GetString(resultbytes);
    }

    internal static byte[] AesDecrypt(byte[] dataBytes, byte[] keyBytes)
    {
        var saltBytes = Encoding.ASCII.GetBytes("iavcpquyxdaxganojgytbfhitzpptqbn");
        using var ms = new MemoryStream();
        //using RijndaelManaged aes = new RijndaelManaged();
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;

        var key = new Rfc2898DeriveBytes(keyBytes, saltBytes, LegacyPbkdf2Iterations, HashAlgorithmName.SHA1);
        aes.Key = key.GetBytes(aes.KeySize / 8);
        aes.IV = key.GetBytes(aes.BlockSize / 8);

        aes.Mode = CipherMode.CBC;

        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cs.Write(dataBytes, 0, dataBytes.Length);
            cs.Close();
        }

        return ms.ToArray();
    }

    public static string Sha256(this string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        return string.Empty;
    }

    public static byte[] Sha256(this byte[] input)
    {
        if (input != null)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(input);
        }

        return Array.Empty<byte>();
    }

    internal static string Sha512(this string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            using var sha = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        return string.Empty;
    }

    internal static string Encode(this byte[] arg)
    {
        var val = Convert.ToBase64String(arg); // Standard base64 encoder
        val = val.Split('=')[0]; // Remove any trailing '='s
        val = val.Replace('+', '-'); // 62nd char of encoding
        val = val.Replace('/', '_'); // 63rd char of encoding
        return val;
    }

    internal static string Encode(List<string> inputs)
    {
        if (inputs.ContainsAny())
        {
            var value = HttpContextExtension.JsonSerialize(inputs);
            var bytes = Encoding.UTF8.GetBytes(value);
            value = bytes.Encode();
            return value;
        }

        return null;
    }

    internal static byte[] Decode(this string input)
    {
        var val = input;
        val = val.Replace('-', '+'); // 62nd char of encoding
        val = val.Replace('_', '/'); // 63rd char of encoding

        switch (val.Length % 4)
        {
            case 0: break; // No pad chars in this case
            case 2: val += "=="; break; // Two pad chars
            case 3: val += "="; break; // One pad char
            default: throw new InvalidOperationException(IllegalStringToDecode);
        }

        return Convert.FromBase64String(val); // Standard base64 decoder
    }

    internal static List<string> DecodeList(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var bytes = value.Decode();
            value = Encoding.UTF8.GetString(bytes);
            var convertedValue = value.JsonDeserialize<string[]>();
            return convertedValue.ToList();
        }

        return new List<string>();
    }

    internal static string GetCrvValueFromCurve(this ECCurve curve)
    {
        return curve.Oid.Value switch
        {
            AuthenticationConstants.CurveOids.P256 => JsonWebKeyECTypes.P256,
            AuthenticationConstants.CurveOids.P384 => JsonWebKeyECTypes.P384,
            AuthenticationConstants.CurveOids.P521 => JsonWebKeyECTypes.P521,
            _ => throw new InvalidOperationException(
                $"Unsupported curve type of {curve.Oid.Value} - {curve.Oid.FriendlyName}")
        };
    }

    internal static string CreateHashClaimValue(this string value, string tokenSigningAlgorithm)
    {
        using var sha = tokenSigningAlgorithm.GetHashAlgorithmForSigningAlgorithm();
        var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(value));
        var size = sha.HashSize / 8 / 2;

        var leftPart = new byte[size];
        Array.Copy(hash, leftPart, size);

        return Base64Url.Encode(leftPart);
    }

    internal static HashAlgorithm GetHashAlgorithmForSigningAlgorithm(this string signingAlgorithm)
    {
        var signingAlgorithmBits = int.Parse(signingAlgorithm.Substring(signingAlgorithm.Length - 3));

        return signingAlgorithmBits switch
        {
            256 => SHA256.Create(),
            384 => SHA384.Create(),
            512 => SHA512.Create(),
            _ => throw new InvalidOperationException(InvalidSigningAlgorithm + signingAlgorithm)
        };
    }

    internal static SecurityKey GetRsaSecurityKey(this string rsaXML)
    {
        if (string.IsNullOrWhiteSpace(rsaXML)) throw new InvalidOperationException(RsaXmlEmpty);

        using var rsa = RSA.Create();
        var keyXml = File.ReadAllText(rsaXML);
        rsa.FromXmlString(keyXml);
        var issuerSigningKey = new RsaSecurityKey(rsa);
        return issuerSigningKey;
    }

    internal static string ComputeSha256Hash(this string rawData)
    {
        // Create a SHA256
        using var sha256Hash = SHA256.Create();

        // ComputeHash - returns byte array
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string
        var builder = new StringBuilder();
        foreach (var t in bytes) builder.Append(t.ToString("x2"));

        return builder.ToString();
    }

    public static string RandomString(this int keyLength)
    {
        using (var rngCryptoServiceProvider = RandomNumberGenerator.Create())
        {
            var randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

    internal static byte[] RandomBytes(this int keySize)
    {
        var saltLength = keySize;
        var ba = new byte[saltLength];
        RandomNumberGenerator.Create().GetBytes(ba);
        return ba;
    }
}
