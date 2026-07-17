namespace ZentraInstallerMVC.Application.Exceptions;

public sealed class InstallationAlreadyCompletedException : Exception
{
    public InstallationAlreadyCompletedException()
        : base("Installation has already completed. Reinstallation is blocked.")
    {
    }
}
