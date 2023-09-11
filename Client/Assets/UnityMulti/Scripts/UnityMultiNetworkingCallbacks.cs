using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class UnityMultiNetworkingCallbacks : MonoBehaviour
{
    
    public UnityMultiNetworking multiNetworking;
    
    public List<string> customMessageTypes { get; private set; } = new List<string>();
    private void OnEnable()
    {
        RegisterCallbacks();
    }

    private void OnDisable()
    {
        UnregisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        multiNetworking = UnityMultiNetworking.Instance;
        if (multiNetworking != null)
        {
            multiNetworking.CustomMessageEvent += OnCustomMessage;
            multiNetworking.ClientErrorEvent += OnClientError;
            multiNetworking.ClientConnectedAndReadyEvent += OnClientConnected;
            multiNetworking.ClientDisconnectedEvent += OnClientDisconnected;
            multiNetworking.ConnectionStateChangeEvent += OnConnectionStateChange;
            multiNetworking.InitialConnectionEvent += OnInitialConnection;
            multiNetworking.MultiErrorEvent += OnValidationError;
            multiNetworking.CreateRoomEvent += OnCreateRoom;
            multiNetworking.JoinRoomEvent += OnJoinRoom;
            multiNetworking.LeaveRoomEvent += OnLeaveRoom;
            multiNetworking.ClientJoinEvent += OnClientJoin;
            multiNetworking.ClientLeaveEvent += OnClientLeave;
        }
    }

    private void UnregisterCallbacks()
    {
        if (multiNetworking != null)
        {
            multiNetworking.CustomMessageEvent -= OnCustomMessage;
            multiNetworking.ClientErrorEvent -= OnClientError;
            multiNetworking.ClientConnectedAndReadyEvent -= OnClientConnected;
            multiNetworking.ClientDisconnectedEvent -= OnClientDisconnected;
            multiNetworking.ConnectionStateChangeEvent -= OnConnectionStateChange;
            multiNetworking.InitialConnectionEvent -= OnInitialConnection;
            multiNetworking.MultiErrorEvent -= OnValidationError;
            multiNetworking.CreateRoomEvent -= OnCreateRoom;
            multiNetworking.JoinRoomEvent -= OnJoinRoom;
            multiNetworking.LeaveRoomEvent -= OnLeaveRoom;
            multiNetworking.ClientJoinEvent -= OnClientJoin;
            multiNetworking.ClientLeaveEvent -= OnClientLeave;
        }
    }

    public virtual void OnClientError(ErrorEventArgs error)
    {

    }
    public virtual void OnClientConnected()
    {

    }
    public virtual void OnClientDisconnected(CloseEventArgs close)
    {
        
    }
    public virtual void OnCustomMessage(Message serverMessage)
    {

    }
    public virtual void OnConnectionStateChange(WebSocketState state)
    {

    }
    public virtual void OnInitialConnection()
    {

    }
    public virtual void OnValidationError(ErrorCode errorCode)
    {

    }
    public virtual void OnCreateRoom(string roomName)
    {

    }
    public virtual void OnJoinRoom(string roomName)
    {

    }
    public virtual void OnLeaveRoom(string roomName)
    {

    }
    public virtual void OnRoomLoad(string roomName)
    {

    }
    public virtual void OnClientJoin(UnityMultiUser user)
    {

    }
    public virtual void OnClientLeave(UnityMultiUser user) { 

    }
}