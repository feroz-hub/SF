using HCL.CS.Domain.Models.Endpoint.Validation;

namespace HCL.CS.Domain.Models.Endpoint.Request;

public class ValidatedEndSessionCallbackRequestModel : ValidatedBaseModel
{
    public IEnumerable<string> FrontChannelLogoutUrls { get; set; }
}
