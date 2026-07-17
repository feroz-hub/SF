namespace HCL.CS.Domain.Entities.Api;

public class UserSecurityQuestions : BaseEntity
{
    public virtual Guid UserId { get; set; }

    public virtual Guid SecurityQuestionId { get; set; }

    public virtual string Answer { get; set; }

    public virtual Users User { get; set; }

    public virtual SecurityQuestions SecurityQuestion { get; set; }
}
