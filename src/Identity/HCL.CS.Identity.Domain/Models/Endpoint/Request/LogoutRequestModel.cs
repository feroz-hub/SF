namespace HCL.CS.Domain.Models.Endpoint.Request;

public class LogoutRequestModel : LogoutMessageModel
{
    public LogoutRequestModel(string endSessionCallBackUrl, LogoutMessageModel message)
    {
        if (message != null)
        {
            ClientId = message.ClientId;
            PostLogoutRedirectUri = message.PostLogoutRedirectUri;
            SubjectId = message.SubjectId;
            SessionId = message.SessionId;
            ClientIdCollection = message.ClientIdCollection;
            Parameters = message.Parameters;
        }

        EndSessionCallBackUrl = endSessionCallBackUrl;
    }

    public string EndSessionCallBackUrl { get; set; }

    public bool ShowSignoutPrompt => string.IsNullOrWhiteSpace(ClientId);
}
