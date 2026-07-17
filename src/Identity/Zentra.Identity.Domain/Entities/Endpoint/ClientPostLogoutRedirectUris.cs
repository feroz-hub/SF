namespace Zentra.Domain.Entities.Endpoint;

public class ClientPostLogoutRedirectUris : BaseEntity
{
    public Guid ClientId { get; set; }

    public string PostLogoutRedirectUri { get; set; }

    public Clients Client { get; set; }
}
