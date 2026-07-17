using Microsoft.AspNetCore.Http;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Parsers;

public interface ITokenParser
{
    Task<string> ParseAsync(HttpContext context);
}
