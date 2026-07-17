using HCL.CS.Domain.Configurations.Api;

namespace TestApp.Helper.Api;

public static class LdapHelper
{
    public static LdapConfig GetTestLdapConfigUnsecure()
    {
        return new LdapConfig
        {
            IsSecureConnection = false,
            LdapHostName = "HCL.CS",
            LdapPort = 10389
        };
    }

    public static LdapConfig GetTestLdapConfigSecure()
    {
        return new LdapConfig
        {
            IsSecureConnection = true,
            LdapHostName = "HCL.CS",
            LdapPort = 10645
        };
    }
}
