using Newtonsoft.Json;

public enum RPCTarget
{
    Buffered = 0,
    All = 1
}

public class UnityMultiRPCInfo
{
    public string MethodName;

    [JsonProperty("Parameters")]
    public object[] Parameters;
    public string ObjName;
    public RPCTarget Target;

    public UnityMultiRPCInfo()
    {
    }

    public UnityMultiRPCInfo(string MethodName, object[] Parameters, string ObjName, RPCTarget Target)
    {
        this.MethodName = MethodName;
        this.Parameters = Parameters;
        this.ObjName = ObjName;
        this.Target = Target;
    }
}