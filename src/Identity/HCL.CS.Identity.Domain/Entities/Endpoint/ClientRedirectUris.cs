namespace HCL.CS.Domain.Entities.Endpoint;

public class ClientRedirectUris : BaseEntity
{
    public Guid ClientId { get; set; }

    public string RedirectUri { get; set; }

    public Clients Client { get; set; }
}
