namespace HCL.CS.SharedKernel;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAtUtc { get; protected set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAtUtc { get; protected set; }

    protected void Touch()
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
