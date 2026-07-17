using System.ComponentModel.DataAnnotations;

namespace Zentra.Domain;

public abstract class BaseModel : BaseTrailModel
{
    public virtual Guid Id { get; set; }
}

public abstract class BaseTrailModel
{
    public string CreatedBy { get; set; }

    public string ModifiedBy { get; set; }

    public virtual DateTime CreatedOn { get; set; }

    public virtual DateTime? ModifiedOn { get; set; }

    public virtual bool IsDeleted { get; set; }

    [Timestamp] public byte[] RowVersion { get; set; }
}

public abstract class BaseResponseModel : BaseModel
{
    public bool IsError { get; set; } = true;

    public string ErrorCode { get; set; }

    public string ErrorDescription { get; set; }
}
