using Newtonsoft.Json;
using UnityEngine;

public class UnityMultiObject : MonoBehaviour
{
    public void SetParams(UnityMultiNetworking multiNetworking, UnityMultiObjectInfo objectInfo)
    {
        this.multiNetworking = multiNetworking;
        this.position = objectInfo.Position;
        this.rotation = objectInfo.Rotation;
        this.scale = objectInfo.Scale;
        this.ObjectID = objectInfo.ObjectID;
        this.Owner = objectInfo.Owner;
    }

    [HideInInspector]
    public UnityMultiNetworking multiNetworking;
    private UnityMultiObjectTransform objectTransform;

    [SerializeField]
    private string ObjectID;
    [SerializeField]
    private string Owner;

    public delegate void TransformUpdateE(Vector3 position, Vector3 scale, Quaternion rotation);
    public event TransformUpdateE TransformUpdate;

    [SerializeField]
    private Vector3 _position;
    public Vector3 position
    {
        get { return _position; }
        set {
            _position = value;
            TransformUpdate?.Invoke(position, scale, rotation);
        }
    }

    [SerializeField]
    private Vector3 _scale;
    public Vector3 scale
    {
        get { return _scale; }
        set
        {
            _scale = value;
            TransformUpdate?.Invoke(position, scale, rotation);
        }
    }

    [SerializeField]
    private Quaternion _rotation;
    public Quaternion rotation
    {
        get { return _rotation; }
        set
        {
            _rotation = value;
            TransformUpdate?.Invoke(position, scale, rotation);
        }
    }

    private void Update()
    {
        position = transform.position;
        scale = transform.localScale;
        rotation = transform.rotation;
    }

    public bool IsMine()
    {
        if (this.Owner == this.multiNetworking.GetUserID()) return true;
        else return false;
    }

    private void Awake()
    {
        this.transform.position = position;
        this.transform.localScale = scale;
        this.transform.rotation = rotation;
        objectTransform = this.gameObject.GetComponent<UnityMultiObjectTransform>();
        if (objectTransform != null)
        {
            objectTransform.Setup(multiNetworking, this);
        }
    }

    public string GetOwner()
    {
        return Owner;
    }

    public string GetObjID()
    {
        return ObjectID;
    }

    public void RunRPC(string methodName, RPCTarget target, params object[] parameters )
    {
        multiNetworking.RPC(this.gameObject, methodName, parameters, target);
    }
}

#nullable enable
public class UnityMultiObjectInfo
{
    public UnityMultiObjectInfo(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale, string Owner)
    {
        this.PrefabName = prefabName;
        this.Position = position;
        this.Rotation = rotation;
        this.Scale = scale;
        this.Owner = Owner;
    }

    [JsonConstructor]
    public UnityMultiObjectInfo(string PrefabName, Vector3 Position, Quaternion Rotation, Vector3 Scale, string Owner, string ObjectID)
    {
        this.PrefabName = PrefabName;
        this.Position = Position;
        this.Rotation = Rotation;
        this.Scale = Scale;
        this.Owner = Owner;
        this.ObjectID = ObjectID;
    }

    public string PrefabName { get; private set; }
    public string? ObjectID { get; private set; }
    public string? Owner { get; private set; }

    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public Vector3 Scale { get; private set; }

}