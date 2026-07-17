using Microsoft.AspNetCore.Http;

namespace HCL.CS.Domain.Models.Endpoint;

public class SecurityEndpointModel
{
    public SecurityEndpointModel(string name, string path, Type handlerType)
    {
        Name = name;
        Path = path;
        Handler = handlerType;
    }

    public PathString Path { get; set; }

    public string Name { get; set; }

    public Type Handler { get; set; }
}
