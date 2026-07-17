namespace HCL.CS.Domain.Models.Endpoint;

public class ClientRedirectUrisModel : BaseModel
{
    public Guid ClientId { get; set; }

    public string RedirectUri { get; set; }
}
