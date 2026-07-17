namespace Zentra.Domain;

public static class GlobalConfiguration
{
    public static bool IsEmailConfigurationValid { get; set; } = false;

    public static bool IsSmsConfigurationValid { get; set; } = false;

    public static bool IsLdapConfigurationValid { get; set; } = false;
}
