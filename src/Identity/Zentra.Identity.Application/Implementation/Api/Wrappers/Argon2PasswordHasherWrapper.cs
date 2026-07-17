using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using Microsoft.AspNetCore.Identity;

namespace Zentra.Service.Implementation.Api.Wrappers;

public class Argon2PasswordHasherWrapper<TUser> : IPasswordHasher<TUser>
    where TUser : class
{
    private readonly int lanes = 5;
    private readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

    public string HashPassword(TUser user, string password)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var salt = new byte[16];

        rng.GetBytes(salt);

        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Version = Argon2Version.Nineteen,
            TimeCost = 10,
            MemoryCost = 32768,
            Lanes = lanes,
            Threads = Environment.ProcessorCount, // higher than "Lanes" doesn't help (or hurt)
            Password = passwordBytes,
            Salt = salt, // >= 8 bytes if not null
            HashLength = 20 // >= 4
        };

        var argon2A = new Argon2(config);
        string hashString;
        using (var hashA = argon2A.Hash())
        {
            hashString = config.EncodeString(hashA.Buffer);
        }

        return hashString;
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(providedPassword);

        var configOfPasswordToVerify = new Argon2Config { Password = passwordBytes, Threads = 1 };
        SecureArray<byte> hashB = null;
        try
        {
            if (configOfPasswordToVerify.DecodeString(hashedPassword, out hashB) && hashB != null)
            {
                var argon2ToVerify = new Argon2(configOfPasswordToVerify);
                using (var hashToVerify = argon2ToVerify.Hash())
                {
                    if (!Argon2.FixedTimeEquals(hashB, hashToVerify)) return PasswordVerificationResult.Failed;
                }
            }
        }
        finally
        {
            hashB?.Dispose();
        }

        if (Argon2.Verify(hashedPassword, passwordBytes, lanes)) return PasswordVerificationResult.Success;

        return PasswordVerificationResult.Failed;
    }
}
