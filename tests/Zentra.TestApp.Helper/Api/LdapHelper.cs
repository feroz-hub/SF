using Zentra.Domain.Configurations.Api;

namespace TestApp.Helper.Api;

public static class LdapHelper
{
    public static LdapConfig GetTestLdapConfigUnsecure()
    {
        return new LdapConfig
        {
            IsSecureConnection = false,
            LdapHostName = "Zentra",
            LdapPort = 10389
        };
    }

    public static LdapConfig GetTestLdapConfigSecure()
    {
        return new LdapConfig
        {
            IsSecureConnection = true,
            LdapHostName = "Zentra",
            LdapPort = 10645
        };
    }
}
