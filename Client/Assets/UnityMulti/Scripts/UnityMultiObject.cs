using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMultiObject : MonoBehaviour
{
    private UnityMultiNetworking multiNetworking;
    public string ObjectName { get; private set; }

    public Vector3 position { get; private set; }
    public Quaternion rotation { get; private set; }
    public Vector3 scale { get; private set; }

    public void SetObjectName(string name)
    {
        ObjectName = name;
    }
}

public class UnityMultiObjectTransform : MonoBehaviour
{
    public bool UpdatePosition = true;
    public bool UpdateRotation = true;
    public bool UpdateScale = false;
}
