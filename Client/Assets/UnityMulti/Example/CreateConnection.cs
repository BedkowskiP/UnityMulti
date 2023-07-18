using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateConnection : UnityMultiNetworkingCallbacks
{

    public string url = "ws://localhost:8080";

    public Button b_create;
    public Button b_join;
    public Button b_connect;

    public InputField i_roomName;
    public InputField i_username;

    private string s_roomName;
    private string s_username;

    public Text isConnected;

    private void Awake()
    {
        b_join.enabled = false;
        b_create.enabled = false;
    }

    public void JoinRoom()
    {
        s_roomName = i_roomName.text;
        if (s_roomName == "")
        {
            Debug.Log("RoomName can't be empty");
            return;
        }
        multiNetworking.JoinRoom(new UnityMultiRoomSettings(RoomName: "RoomTestA", Password: ""));
    }

    public void CreateRoom()
    {

        s_roomName = i_roomName.text;
        if (s_roomName == "")
        {
            Debug.Log("RoomName can't be empty");
            return;
        }
        multiNetworking.CreateRoom(new UnityMultiRoomSettings(RoomName: "RoomTestA", Password: "", SceneName: "TutorialSceneTwo"));
    }

    public void Connect()
    {
        s_username = i_username.text;
        multiNetworking.ConnectToServer(url, s_username);

        b_create.enabled = true;
        b_join.enabled = true;
        b_connect.enabled = false;
    }

    public override void OnClientConnected()
    {
        base.OnClientConnected();
        isConnected.text = "Connected";
    }
}
