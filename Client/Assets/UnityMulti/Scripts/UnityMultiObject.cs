using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class UnityMultiObject : MonoBehaviour
{
    public void SetParams(UnityMultiNetworking multiNetworking, UnityMultiObjectInfo objectInfo)
    {
        this.multiNetworking = multiNetworking;
        this.position = objectInfo.Position.GetVec3();
        this.rotation = objectInfo.Rotation.GetQuat();
        this.scale = objectInfo.Scale.GetVec3();
        this.ObjectID = objectInfo.ObjectID;
        this.Owner = objectInfo.Owner;
    }

    private UnityMultiNetworking multiNetworking;
    private UnityMultiObjectTransform objectTransform;

    [SerializeField]
    private string ObjectID;
    [SerializeField]
    private string Owner;

    public delegate void Vec3E(Vector3 value, Vector3 newValue);
    public event Vec3E UpdatePosition;
    public event Vec3E UpdateScale;

    public delegate void QuatE(Quaternion value, Quaternion newValue);
    public event QuatE UpdateRotation;

    private Vector3 _position;
    public Vector3 position
    {
        get { return _position; }
        set {
            UpdatePosition?.Invoke(position, value);
            _position = value;
            try { this.gameObject.transform.position = position; } catch { }
        }
    }

    private Vector3 _scale;
    public Vector3 scale
    {
        get { return _scale; }
        set
        {
            UpdateScale?.Invoke(scale, value);
            _scale = value;
            try { this.gameObject.transform.localScale = scale; } catch { }
        }
    }

    private Quaternion _rotation;
    public Quaternion rotation
    {
        get { return _rotation; }
        set
        {
            UpdateRotation?.Invoke(rotation, value);
            _rotation = value;
            try { this.gameObject.transform.rotation = rotation; } catch { }
        }
    }

    public bool IsMine()
    {
        if (Owner == multiNetworking.clientData.UserID) return true;
        else return false;
    }

    private void Awake()
    {
        objectTransform = this.gameObject.GetComponent<UnityMultiObjectTransform>();
        if (objectTransform != null)
        {
            objectTransform.Setup(multiNetworking, this);
        }
    }

}
