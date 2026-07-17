using Microsoft.AspNetCore.Http;
using Zentra.Domain.Models.Endpoint;

namespace Zentra.Service.Interfaces.Interfaces.Endpoint.Parsers;

public interface IClientSecretParser
{
    Task<ParsedSecretModel> ParseAsync(HttpContext context);
}
