using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
[System.Serializable]
public class UnityMultiObjectInfo
{
    public UnityMultiObjectInfo(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale, GameObject parent, string OwnerID)
    {
        this.PrefabName = prefabName;
        this.Position = new ObjVec3(position);
        this.Rotation = new ObjQuat(rotation);
        this.Scale = new ObjVec3(scale);
        if (parent == null) this.ParentObject = null;
        else this.ParentObject = parent.name;
        this.Owner = OwnerID;
    }

    [JsonConstructor]
    public UnityMultiObjectInfo(string PrefabName, ObjVec3 Position, ObjQuat Rotation, ObjVec3 Scale, string Owner, string ObjectID, string ParentObject)
    {
        this.PrefabName = PrefabName;
        this.Position = Position;
        this.Rotation = Rotation;
        this.Scale = Scale;
        this.Owner = Owner;
        this.ObjectID = ObjectID;
        this.ParentObject = ParentObject;
    }

    public string PrefabName { get; private set; }
    public string? ObjectID { get; private set; }
    public string? Owner { get; private set; }
    public string? ParentObject { get; private set; }

    public ObjVec3 Position { get; private set; }
    public ObjQuat Rotation { get; private set; }
    public ObjVec3 Scale { get; private set; }

}

public class ObjVec3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public ObjVec3(Vector3 vec)
    {
        this.x = vec.x;
        this.y = vec.y;
        this.z = vec.z;
    }

    [JsonConstructor]
    public ObjVec3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVec3()
    {
        return new Vector3(x, y, z);
    }
}

public class ObjQuat
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float w { get; set; }

    public ObjQuat(Quaternion quat)
    {
        this.x = quat.x;
        this.y = quat.y;
        this.z = quat.z;
        this.w = quat.w;
    }

    [JsonConstructor]
    public ObjQuat(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Quaternion GetQuat()
    {
        return new Quaternion(x, y, z, w);
    }
}
