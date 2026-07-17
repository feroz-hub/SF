namespace HCL.CS.Domain.Models.Endpoint;

public class ClientPostLogoutRedirectUrisModel : BaseModel
{
    public Guid ClientId { get; set; }

    public string PostLogoutRedirectUri { get; set; }
}
