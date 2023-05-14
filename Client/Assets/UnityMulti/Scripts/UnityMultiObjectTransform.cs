using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityMultiObject))]
public class UnityMultiObjectTransform : MonoBehaviour
{
    private UnityMultiNetworking multiNetworking;
    private UnityMultiObject parentObj;

    public void Setup(UnityMultiNetworking multiNetworking, UnityMultiObject parentObj)
    {
        this.multiNetworking = multiNetworking;
        this.parentObj = parentObj;
    }

    private void Awake()
    {
        parentObj.UpdatePosition += OnUpdatePosition;
        parentObj.UpdateRotation += OnUpdateRotation;
        parentObj.UpdateScale += OnUpdateScale;
    }

    private void OnUpdatePosition(Vector3 oldValue, Vector3 newValue)
    {
        if(newValue != oldValue)
        {

        }
    }

    private void OnUpdateRotation(Quaternion oldValue, Quaternion newValue)
    {

    }

    private void OnUpdateScale(Vector3 oldValue, Vector3 newValue)
    {

    }
}
