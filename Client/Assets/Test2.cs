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
        settings = new UnityMultiRoomSettings("RoomName");
        multiNetworking.CreateRoom(settings);
    }

    // Update is called once per frame
    void Update()
    {
        //this.ms = multiNetworking.GetLatency();
    }

    public override void OnClientConnected()
    {
        base.OnClientConnected();

        settings = new UnityMultiRoomSettings("RoomName");
        
        Debug.Log(multiNetworking.clientData.Username);
        Debug.Log(multiNetworking.clientData.UserID);
        multiNetworking.CreateRoom(settings);
    }

}
