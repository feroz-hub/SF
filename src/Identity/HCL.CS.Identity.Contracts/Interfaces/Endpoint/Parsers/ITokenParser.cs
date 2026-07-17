using Microsoft.AspNetCore.Http;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Parsers;

public interface ITokenParser
{
    Task<string> ParseAsync(HttpContext context);
}
