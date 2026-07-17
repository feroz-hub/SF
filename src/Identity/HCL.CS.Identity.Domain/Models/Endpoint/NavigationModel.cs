using HCL.CS.Domain.Models.Endpoint.Response;

namespace HCL.CS.Domain.Models.Endpoint;

public class NavigationModel : ErrorResponseModel
{
    public bool IsLogin { get; set; }

    public bool IsRedirect => !string.IsNullOrWhiteSpace(RedirectUrl);

    public string RedirectUrl { get; set; }
}
