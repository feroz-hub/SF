namespace HCL.CS.Domain.Models.Endpoint;

public class ResourceScopeModel
{
    public Dictionary<string, string> RawData { get; set; }

    public List<string> RequestedScope { get; set; }

    public ClientsModel Client { get; set; }

    public string UserName { get; set; }
}
