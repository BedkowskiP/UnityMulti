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

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 targetScale;

    private bool isSendingTransform = false;
    private int maxUpdatesPerSec = 60;
    private float smoothSpeed = 10f;


    public void Setup(UnityMultiNetworking multiNetworking, UnityMultiObject Obj)
    {
        this.multiNetworking = multiNetworking;
        this.Obj = Obj;
    }

    private Vector3 positionToSend;
    private Vector3 scaleToSend;
    private Quaternion rotationToSend;

    private void Awake()
    {
        Obj.TransformUpdate += OnTransformUpdate;

        positionToSend = transform.position;
        scaleToSend = transform.localScale;
        rotationToSend = transform.rotation;

        targetPosition = transform.position;
        targetRotation = transform.rotation;
        targetScale = transform.localScale;
    }

    private void OnTransformUpdate(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        if (Obj.IsMine())
        {
            bool transformUpdated = false;
            if (updatePosition)
            {
                if (positionToSend != position) 
                {
                    transformUpdated = true;
                    positionToSend = position;
                }
                
            }

            if (updateScale)
            {
                if (scaleToSend != scale)
                {
                    transformUpdated = true;
                    scaleToSend = scale;
                }
                
            }

            if (updateRotation)
            {
                if (rotationToSend != rotation) 
                {
                    transformUpdated = true;
                    rotationToSend = rotation;
                }
                
            }

            if(transformUpdated)
                if (!isSendingTransform)
                {
                    StartCoroutine(SendTransformCoroutine());
                }
        }
    }

    private void Update()
    {
        if (!Obj.IsMine())
        {
            if (updatePosition)
            {
                if (targetPosition != transform.position)
                    transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            }

            if (updateRotation)
            {
                if (targetRotation != transform.rotation)
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
            }

            if (updateScale)
            {
                if (targetScale != transform.localScale)
                    transform.localScale = Vector3.Lerp(transform.localScale, targetScale, smoothSpeed * Time.deltaTime);
            }
        }
        
    }

    private void SendNewTransform()
    {
        UnityMultiTransformInfo transformInfo = new UnityMultiTransformInfo(this.name, Obj.GetObjID(), positionToSend, scaleToSend, rotationToSend);
        Message newTransform = new Message(MessageType.TRANSFORM_UPDATE, JsonConvert.SerializeObject(transformInfo));
        multiNetworking.SendMessage(newTransform);
    }

    private IEnumerator SendTransformCoroutine()
    {
        isSendingTransform = true;
        SendNewTransform();
        yield return new WaitForSeconds(1f / maxUpdatesPerSec);
        isSendingTransform = false;
    }

    public void UpdateTransform(UnityMultiTransformInfo transformUpdate)
    {
        if (updatePosition)
        {
            targetPosition = transformUpdate.Position.GetVec3();
        }

        if (updateRotation)
        {
            targetRotation = transformUpdate.Rotation.GetQuat();
        }

        if (updateScale)
        {
            targetScale = transformUpdate.Scale.GetVec3();
        }
    }
}

#nullable enable
public class UnityMultiTransformInfo
{
    public ObjVec3 Position { get; private set; }
    public ObjQuat Rotation { get; private set; }
    public ObjVec3 Scale { get; private set; }

    public string ObjName { get; private set; }
    public string ObjID { get; private set; }
    public string? OwnerID { get; private set; }

    public UnityMultiTransformInfo(string ObjName, string ObjID, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        this.Position = new ObjVec3(position);
        this.Rotation = new ObjQuat(rotation);
        this.Scale = new ObjVec3(scale);
        this.ObjName = ObjName;
        this.ObjID = ObjID;
    }

    [JsonConstructor]
    public UnityMultiTransformInfo(string ObjName, string ObjID, string OwnerID, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        this.Position = new ObjVec3(position);
        this.Rotation = new ObjQuat(rotation);
        this.Scale = new ObjVec3(scale);
        this.ObjName = ObjName;
        this.ObjID = ObjID;
        this.OwnerID = OwnerID;
    }
}

