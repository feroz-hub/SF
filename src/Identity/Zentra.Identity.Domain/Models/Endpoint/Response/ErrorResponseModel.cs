using System.ComponentModel;

namespace Zentra.Domain.Models.Endpoint.Response;

public class ErrorResponseModel
{
    public bool IsError { get; set; } = true;

    [DisplayName("error")] public string ErrorCode { get; set; }

    [DisplayName("error_description")] public string ErrorDescription { get; set; }
}

public class ErrorResponseResultModel
{
    public string error { get; set; }

    public string error_description { get; set; }
}
