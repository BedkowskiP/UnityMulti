using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class ExampleEventHandler : UnityMultiNetworkingCallbacks
{
    public void Disconnect()
    {
        multiNetworking.Disconnect();
    }

    public void LeaveRoom()
    {
        multiNetworking.LeaveRoom();
    }

    public override void OnClientDisconnected(CloseEventArgs close)
    {
        base.OnClientDisconnected(close);
        if(close.Code == 1000)
        {
            multiNetworking.LoadLevel("TutorialSceneOne");
        }
    }
    public override void OnLeaveRoom(string roomName)
    {
        base.OnLeaveRoom(roomName);
        multiNetworking.LoadLevel("TutorialSceneOne");
    }
}
