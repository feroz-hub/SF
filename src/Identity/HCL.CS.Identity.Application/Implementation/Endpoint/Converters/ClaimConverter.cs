/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using Newtonsoft.Json;

namespace HCL.CS.Service.Implementation.Endpoint.Converters;

internal class ClaimConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(Claim) == objectType;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var source = serializer.Deserialize<ClaimObject>(reader);
        var target = new Claim(source.Type, source.Value, source.ValueType);
        return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var source = (Claim)value;

        var target = new ClaimObject
        {
            Type = source.Type,
            Value = source.Value,
            ValueType = source.ValueType
        };

        serializer.Serialize(writer, target);
    }
}

public class ClaimObject
{
    public string Type { get; set; }

    public string Value { get; set; }

    public string ValueType { get; set; }
}
