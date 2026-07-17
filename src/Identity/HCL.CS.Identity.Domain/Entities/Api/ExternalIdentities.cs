namespace HCL.CS.Domain.Entities.Api;

public class ExternalIdentities : BaseEntity
{
    public Guid UserId { get; set; }

    public string TenantId { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool EmailVerified { get; set; }

    public DateTime LinkedAt { get; set; }

    public DateTime? LastSignInAt { get; set; }
}
