using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : UnityMultiNetworkingCallbacks
{
    public string url = "ws://localhost:8080";
    public long ms;
    UnityMultiRoomSettings settings;
    void Start()
    {
        multiNetworking.Connect(url, "betek");
        settings = new UnityMultiRoomSettings("RoomNames");
    }

    void Update()
    {
        this.ms = multiNetworking.GetLatency();
    }

    public override void OnClientConnected()
    {
        base.OnClientConnected();        
        multiNetworking.CreateRoom(settings);
    }

    public override void OnCreateRoom(string roomName)
    {
        base.OnCreateRoom(roomName);
        multiNetworking.JoinRoom(settings);
    }
}
