using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnityMultiObjectInfo
{
    [HideInInspector]
    public string PrefabName { get; private set; }

    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

    public float RotX { get; set; }
    public float RotY { get; set; }
    public float RotZ { get; set; }
    public float RotW { get; set; }

    public float ScalX { get; set; }
    public float ScalY { get; set; }
    public float ScalZ { get; set; }
    public UnityMultiObjectInfo(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.PrefabName = prefabName;

        PosX = position.x;
        PosY = position.y;
        PosZ = position.z;

        ScalX = scale.x;
        ScalY = scale.y;
        ScalZ = scale.z;

        RotX = rotation.x;
        RotY = rotation.y;
        RotZ = rotation.z;
        RotW = rotation.w;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(PosX, PosY, PosZ);
    }
    public Vector3 GetScale()
    {
        return new Vector3(ScalX, ScalY, ScalZ);
    }
    public Quaternion GetRotation()
    {
        return new Quaternion(RotX, RotY, RotZ, RotW);
    }
}


