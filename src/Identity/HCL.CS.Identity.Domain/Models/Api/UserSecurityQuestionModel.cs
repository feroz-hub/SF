namespace HCL.CS.Domain.Models.Api;

public class UserSecurityQuestionModel : BaseModel
{
    public virtual Guid UserId { get; set; }

    public virtual Guid SecurityQuestionId { get; set; }

    public virtual string Answer { get; set; }
}
