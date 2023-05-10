using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnityMultiObjectInfo
{
    [HideInInspector]
    public string prefabName { get; private set; }

    private float posX { get; set; }
    private float posY { get; set; }
    private float posZ { get; set; }

    private float rotX { get; set; }
    private float rotY { get; set; }
    private float rotZ { get; set; }
    private float rotW { get; set; }

    private float scalX { get; set; }
    private float scalY { get; set; }
    private float scalZ { get; set; }
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


