using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnityMultiObjectInfo
{
    [HideInInspector]
    public string prefabName { get; private set; }

    public float posX { get; set; }
    public float posY { get; set; }
    public float posZ { get; set; }

    public float rotX { get; set; }
    public float rotY { get; set; }
    public float rotZ { get; set; }
    public float rotW { get; set; }

    public float scalX { get; set; }
    public float scalY { get; set; }
    public float scalZ { get; set; }
    public UnityMultiObjectInfo(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.prefabName = prefabName;

        posX = position.x;
        posY = position.y;
        posZ = position.z;

        scalX = scale.x;
        scalY = scale.y;
        scalZ = scale.z;

        rotX = rotation.x;
        rotY = rotation.y;
        rotZ = rotation.z;
        rotW = rotation.w;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(posX, posY, posZ);
    }
    public Vector3 GetScale()
    {
        return new Vector3(scalX, scalY, scalZ);
    }
    public Quaternion GetRotation()
    {
        return new Quaternion(rotX, rotY, rotZ, rotW);
    }
}


