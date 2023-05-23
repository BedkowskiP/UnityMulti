using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[RequireComponent(typeof(UnityMultiObject))]
public class UnityMultiObjectTransform : MonoBehaviour
{
    private UnityMultiNetworking multiNetworking;
    private UnityMultiObject Obj;

    public bool updatePosition = true;
    public bool updateRotation = true;
    public bool updateScale = false;

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
        if (!updatePosition) return;
        if (Obj.IsMine())
        {
            if (newValue != oldValue)
            {
                SendNewTransform(newValue, Obj.rotation, Obj.scale);
            }
        }
    }

    private void OnUpdateRotation(Quaternion oldValue, Quaternion newValue)
    {
        if (!updateRotation) return;
        if (Obj.IsMine())
        {
            if (newValue != oldValue)
            {
                SendNewTransform(Obj.position, newValue, Obj.scale);
            }
        }
    }

    private void OnUpdateScale(Vector3 oldValue, Vector3 newValue)
    {
        if (!updateScale) return;
        if (Obj.IsMine())
        {
            if (newValue != oldValue)
            {
                SendNewTransform(Obj.position, Obj.rotation, newValue);
            }
        }
    }

    private void SendNewTransform(Vector3 pos, Quaternion rot, Vector3 scal)
    {
        //UnityMultiTransformInfo transformInfo = new UnityMultiTransformInfo(pos, rot,scal, Obj.GetID());
        //Message newTransform = new Message(MessageType.TRANSFORM_UPDATE, JsonConvert.SerializeObject(transformInfo));
        //multiNetworking.SendMessage(newTransform);
    }
}

public class UnityMultiTransformInfo
{

}

