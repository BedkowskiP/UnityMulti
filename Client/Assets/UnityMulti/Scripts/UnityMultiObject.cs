using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMultiObject : MonoBehaviour
{
    public UnityMultiObject(UnityMultiNetworking multiNetworking, UnityMultiObjectInfo objectInfo)
    {
        this.multiNetworking = multiNetworking;
        this.position = objectInfo.Position.GetVec3();
        this.rotation = objectInfo.Rotation.GetQuat();
        this.scale = objectInfo.Scale.GetVec3();
        this.ObjectID = objectInfo.ObjectID;
        this.Owner = objectInfo.Owner;
    }

    [HideInInspector]
    private UnityMultiNetworking multiNetworking;

    [SerializeField]
    private string ObjectID;
    [SerializeField]
    private string Owner;

    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;

    public bool IsMine()
    {
        if (Owner == multiNetworking.clientData.UserID) return true;
        else return false;
    }
    


}
