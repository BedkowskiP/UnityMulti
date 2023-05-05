using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class UnityMultiNetworkingCallbacks : MonoBehaviour
{
    public UnityMultiNetworking multiNetworking;
    [HideInInspector]
    public List<string> customMessageTypes { get; private set; } = new List<string>();

    public WebSocketState connectionState;

    private void Awake()
    {
        multiNetworking = UnityMultiNetworking.Instance;
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

    public virtual void OnClientError(ErrorEventArgs error)
    {
        Debug.LogError(
            "Error Exception: " + error.Exception +
            "\nError Message: " + error.Message
            );
    }

    public virtual void OnClientConnected()
    {
        Debug.Log("Connected to server and ready to join room.");
    }

    public virtual void OnClientDisconnected(CloseEventArgs close)
    {
        Debug.Log("Disconnected from server.");
    }

    public virtual void OnCustomMessage(Message serverMessage)
    {
        // Handle custom message type
    }

    public virtual void OnConnectionStateChange()
    {
        connectionState = multiNetworking.getState();
    }

    public virtual void OnInitialConnection()
    {
        Debug.Log("Validating user data");
    }

    public virtual void OnValidationError(ErrorCode errorCode)
    {
        Debug.Log("Server error: ErrorCode: " + errorCode + " | ErrorMessage: " + UnityMultiErrorHandler.ErrorMessage(errorCode));
    }

    public virtual void OnCreateRoom(string roomName)
    {
        Debug.Log("Room: '" + roomName + "' created succesfully.");
    }
    public virtual void OnJoinRoom(string roomName)
    {
        Debug.Log("Joined room '" + roomName + "' succesfully.");
    }

    public virtual void OnLeaveRoom(string roomName)
    {
        Debug.Log("You left room '" + roomName + "' succesfully");
    }
    public virtual void OnClientJoin(UnityMultiUser user)
    {
        Debug.Log("User: " + user.Username + " joined the room");
    }
    public virtual void OnClientLeave(UnityMultiUser user)
    {
        Debug.Log("User: " + user.Username + " left the room");
    }
}