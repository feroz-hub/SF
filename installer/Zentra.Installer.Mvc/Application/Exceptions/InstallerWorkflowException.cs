namespace ZentraInstallerMVC.Application.Exceptions;

public sealed class InstallerWorkflowException : Exception
{
    public InstallerWorkflowException(string message)
        : base(message)
    {
    }
}
