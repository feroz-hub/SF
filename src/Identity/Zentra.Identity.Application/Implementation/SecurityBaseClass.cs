using Zentra.Service.Interfaces;

namespace Zentra.Service.Implementation;
// TODO: Need to fix warning S3881: Fix implementation of IDisposable to conform to Dispose pattern.

public abstract class SecurityBase : IBaseClass
{
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SecurityBase()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
