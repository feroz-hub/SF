using System.Net;

namespace IntegrationTests.Endpoint.Setup;

public class UserAgent : HttpClient
{
    public UserAgent(UserAgentHandler userAgentHandler)
        : base(userAgentHandler)
    {
        UserAgentHandler = userAgentHandler;
    }

    public UserAgentHandler UserAgentHandler { get; }

    public bool AllowCookies
    {
        get => UserAgentHandler.AllowCookies;
        set => UserAgentHandler.AllowCookies = value;
    }

    public bool AllowAutoRedirect
    {
        get => UserAgentHandler.AllowAutoRedirect;
        set => UserAgentHandler.AllowAutoRedirect = value;
    }

    public int ErrorRedirectLimit
    {
        get => UserAgentHandler.ErrorRedirectLimit;
        set => UserAgentHandler.ErrorRedirectLimit = value;
    }

    public int StopRedirectingAfter
    {
        get => UserAgentHandler.StopRedirectingAfter;
        set => UserAgentHandler.StopRedirectingAfter = value;
    }

    internal void RemoveCookie(string uri, string name)
    {
        UserAgentHandler.RemoveCookie(uri, name);
    }

    internal Cookie GetCookie(string uri, string name)
    {
        return UserAgentHandler.GetCookie(uri, name);
    }
}
