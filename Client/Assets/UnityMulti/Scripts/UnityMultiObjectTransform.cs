using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityMultiObject))]
public class UnityMultiObjectTransform : MonoBehaviour
{
    private UnityMultiNetworking multiNetworking;
    private UnityMultiObject Obj;

    public void Setup(UnityMultiNetworking multiNetworking, UnityMultiObject Obj)
    {
        this.multiNetworking = multiNetworking;
        this.Obj = Obj;
    }

    private void Awake()
    {
        Obj.UpdatePosition += OnUpdatePosition;
        Obj.UpdateRotation += OnUpdateRotation;
        Obj.UpdateScale += OnUpdateScale;
    }

    private void OnUpdatePosition(Vector3 oldValue, Vector3 newValue)
    {
        if(newValue != oldValue)
        {
            if (Obj.IsMine())
            {
                
            }
        }
    }

    private void OnUpdateRotation(Quaternion oldValue, Quaternion newValue)
    {

    }

    private void OnUpdateScale(Vector3 oldValue, Vector3 newValue)
    {

    }
}
