using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateConnection : UnityMultiNetworkingCallbacks
{
    public string url = "ws://localhost:8080";
    public long ms;
    UnityMultiRoomSettings settings;
    void Start()
    {
        multiNetworking.Connect(url, "Betek");
        settings = new UnityMultiRoomSettings(RoomName: "RoomNames", Password:"", SceneName:"TutorialSceneTwo");
    }

    void Update()
    {
        ms = multiNetworking.GetLatency();
    }

    public override void OnClientConnected()
    {
        base.OnClientConnected();
        //multiNetworking.room.CreateRoom(settings);
        multiNetworking.room.JoinRoom(settings);
    }
}
