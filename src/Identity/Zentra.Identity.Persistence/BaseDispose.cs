namespace Zentra.Infrastructure.Data;

public abstract class BaseDispose : IDisposable
{
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
