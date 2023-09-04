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
            SceneManager.LoadScene(multiNetworking.startingScene);
        }
    }
    public override void OnLeaveRoom(string roomName)
    {
        base.OnLeaveRoom(roomName);
        SceneManager.LoadScene(multiNetworking.startingScene);
    }
}
