using Zentra.Domain.Models.Endpoint;

namespace Zentra.Domain.Models;

public class BackChannelLogoutModel
{
    public string ClientId { get; set; }

    public string SubjectId { get; set; }

    public string SessionId { get; set; }

    public string LogoutUri { get; set; }

    public bool SessionIdRequired { get; set; }

    public ClientsModel Client { get; set; }
}
