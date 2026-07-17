using System.Security.Claims;
using Newtonsoft.Json;
using HCL.CS.Domain.Constants.Endpoint;

namespace HCL.CS.Service.Implementation.Endpoint.Converters;

internal class ClaimsPrincipalConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(ClaimsPrincipal) == objectType;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var source = serializer.Deserialize<ClaimsPrincipalObject>(reader);
        if (source == null) return null;

        var claims = source.Claims.Select(x => new Claim(x.Type, x.Value, x.ValueType));
        var id = new ClaimsIdentity(claims, source.AuthenticationType, OpenIdConstants.ClaimTypes.Name,
            OpenIdConstants.ClaimTypes.Role);
        var target = new ClaimsPrincipal(id);
        return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var source = (ClaimsPrincipal)value;

        var target = new ClaimsPrincipalObject
        {
            AuthenticationType = source.Identity.AuthenticationType,
            Claims = source.Claims.Select(x => new ClaimObject
                { Type = x.Type, Value = x.Value, ValueType = x.ValueType }).ToArray()
        };
        serializer.Serialize(writer, target);
    }
}

public class ClaimsPrincipalObject
{
    public string AuthenticationType { get; set; }

    public ClaimObject[] Claims { get; set; }
}
