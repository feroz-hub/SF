using Zentra.Domain.Models.Endpoint.Validation;

namespace Zentra.Domain.Models.Endpoint.Request;

public class ValidatedEndSessionCallbackRequestModel : ValidatedBaseModel
{
    public IEnumerable<string> FrontChannelLogoutUrls { get; set; }
}
