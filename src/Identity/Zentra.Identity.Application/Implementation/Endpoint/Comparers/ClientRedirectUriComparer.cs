using Zentra.Domain.Entities.Endpoint;

namespace Zentra.Service.Implementation.Endpoint.Comparers;

internal class ClientRedirectUriComparer : IEqualityComparer<ClientRedirectUris>
{
    public bool Equals(ClientRedirectUris x, ClientRedirectUris y)
    {
        if (x.ClientId == y.ClientId && x.RedirectUri.ToLower() == y.RedirectUri.ToLower()) return true;

        return false;
    }

    public int GetHashCode(ClientRedirectUris obj)
    {
        return obj.Id.GetHashCode();
    }
}
