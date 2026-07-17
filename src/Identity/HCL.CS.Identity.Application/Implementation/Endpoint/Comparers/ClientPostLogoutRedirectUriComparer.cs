using HCL.CS.Domain.Entities.Endpoint;

namespace HCL.CS.Service.Implementation.Endpoint.Comparers;

internal class ClientPostLogoutRedirectUriComparer : IEqualityComparer<ClientPostLogoutRedirectUris>
{
    public bool Equals(ClientPostLogoutRedirectUris x, ClientPostLogoutRedirectUris y)
    {
        if (x.ClientId == y.ClientId &&
            x.PostLogoutRedirectUri.ToLower() == y.PostLogoutRedirectUri.ToLower()) return true;

        return false;
    }

    public int GetHashCode(ClientPostLogoutRedirectUris obj)
    {
        return obj.Id.GetHashCode();
    }
}
