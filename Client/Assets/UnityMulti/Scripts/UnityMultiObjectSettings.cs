using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMultiObjectToInstantiate : MonoBehaviour
{
    private string prefabName;
    public Vector3 position { get; private set; }
    public Quaternion rotation { get; private set; }
    public Vector3 scale { get; private set; }

    public UnityMultiObjectToInstantiate(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.prefabName = prefabName;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}

