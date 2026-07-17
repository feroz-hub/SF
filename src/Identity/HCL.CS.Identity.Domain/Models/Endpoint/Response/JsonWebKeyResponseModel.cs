using System.Text.Json.Serialization;

namespace HCL.CS.Domain.Models.Endpoint.Response;

public class JsonWebKeyResponseModel
{
    [JsonPropertyName("kty")] public string Kty { get; set; }

    [JsonPropertyName("use")] public string Use { get; set; }

    [JsonPropertyName("kid")] public string Kid { get; set; }

    [JsonPropertyName("x5t")] public string X5t { get; set; }

    [JsonPropertyName("e")] public string E { get; set; }

    [JsonPropertyName("n")] public string N { get; set; }

    [JsonPropertyName("x5c")] public IList<string> X5c { get; set; }

    [JsonPropertyName("alg")] public string Alg { get; set; }

    [JsonPropertyName("x")] public string X { get; set; }

    [JsonPropertyName("y")] public string Y { get; set; }

    [JsonPropertyName("crv")] public string Crv { get; set; }
}
