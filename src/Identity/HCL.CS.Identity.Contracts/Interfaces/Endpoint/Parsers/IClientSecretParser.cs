using Microsoft.AspNetCore.Http;
using HCL.CS.Domain.Models.Endpoint;

namespace HCL.CS.Service.Interfaces.Interfaces.Endpoint.Parsers;

public interface IClientSecretParser
{
    Task<ParsedSecretModel> ParseAsync(HttpContext context);
}
