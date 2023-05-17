using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateConnection : UnityMultiNetworkingCallbacks
{
    public string url = "ws://localhost:8080";
    public long ms;
    UnityMultiRoomSettings settings;

public string username;
    void Start()
    {
    }

    void Update()
    {
        ms = multiNetworking.GetLatency();
    }

    public override void OnClientConnected()
    {
        base.OnClientConnected();
        Debug.Log("Joining room");
        //multiNetworking.CreateRoom(settings);
        
    }
    void OnGUI() {
        if (GUILayout.Button("connect"))
        {
            multiNetworking.Connect(url, username);
            settings = new UnityMultiRoomSettings(RoomName: "RoomNames", Password:"", SceneName:"TutorialSceneTwo");
        }
        if (GUILayout.Button("create"))multiNetworking.CreateRoom(settings);
        if (GUILayout.Button("join"))multiNetworking.JoinRoom(settings);
        
        
        
    }
}
