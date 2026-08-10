/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
