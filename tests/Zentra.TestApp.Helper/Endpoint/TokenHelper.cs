using Zentra.Domain.Models.Endpoint.Request;

namespace TestApp.Helper.Endpoint;

public static class TokenHelper
{
    public static ValidatedTokenRequestModel GetValidatedTokenRequestModel()
    {
        var validatedTokenRequestModel = new ValidatedTokenRequestModel();
        return validatedTokenRequestModel;
    }
}
