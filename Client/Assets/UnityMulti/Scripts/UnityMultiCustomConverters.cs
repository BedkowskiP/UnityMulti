using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomConverters
{
    public static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new List<JsonConverter>
            {
                new Vector3Converter(),
                new QuaternionConverter(),
                new ColorConverter(),
                new UnityMultiRPCInfoConverter()
            }
    };
}

public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        JToken.FromObject(new { x = value.x, y = value.y, z = value.z }).WriteTo(writer);
    }

    public override Vector3 ReadJson(JsonReader reader, System.Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        float x = jsonObject["x"].Value<float>();
        float y = jsonObject["y"].Value<float>();
        float z = jsonObject["z"].Value<float>();
        return new Vector3(x, y, z);
    }
}

public class QuaternionConverter : JsonConverter<Quaternion>
{
    public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
    {
        JToken.FromObject(new { x = value.x, y = value.y, z = value.z, w = value.w }).WriteTo(writer);
    }

    public override Quaternion ReadJson(JsonReader reader, System.Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        float x = jsonObject["x"].Value<float>();
        float y = jsonObject["y"].Value<float>();
        float z = jsonObject["z"].Value<float>();
        float w = jsonObject["w"].Value<float>();
        return new Quaternion(x, y, z, w);
    }
}

public class ColorConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        JToken.FromObject(new { r = value.r, g = value.g, b = value.b, a = value.a }).WriteTo(writer);
    }

    public override Color ReadJson(JsonReader reader, System.Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        float r = jsonObject["r"].Value<float>();
        float g = jsonObject["g"].Value<float>();
        float b = jsonObject["b"].Value<float>();
        float a = jsonObject["a"].Value<float>();
        return new Color(r, g, b, a);
    }
}

public class UnityMultiRPCInfoConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(UnityMultiRPCInfo);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        UnityMultiRPCInfo rpcInfo = new UnityMultiRPCInfo();
        rpcInfo.MethodName = jsonObject["MethodName"].ToString();
        rpcInfo.Parameters = jsonObject["Parameters"].ToObject<object[]>();
        rpcInfo.ObjName = jsonObject["ObjName"].ToString();
        int targetValue = jsonObject["Target"].Value<int>();
        rpcInfo.Target = (RPCTarget)targetValue;
        return rpcInfo;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        UnityMultiRPCInfo rpcInfo = (UnityMultiRPCInfo)value;

        JToken parametersToken = JToken.FromObject(rpcInfo.Parameters, JsonSerializer.Create(CustomConverters.settings));

        JObject jsonObject = new JObject();
        jsonObject["MethodName"] = rpcInfo.MethodName;
        jsonObject["Parameters"] = parametersToken;
        jsonObject["ObjName"] = rpcInfo.ObjName;
        jsonObject["Target"] = (int)rpcInfo.Target;

        jsonObject.WriteTo(writer);
    }

}