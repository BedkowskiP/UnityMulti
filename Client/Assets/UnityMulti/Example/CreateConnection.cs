using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateConnection : UnityMultiNetworkingCallbacks
{

    public string url = "ws://localhost:8080";
    UnityMultiRoomSettings settings;

    public Button b_create;
    public Button b_join;
    public Button b_connect;

    public InputField i_roomName;
    public InputField i_username;

    private string s_roomName;
    private string s_username;

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
        settings = new UnityMultiRoomSettings(RoomName: s_roomName, Password: "", SceneName: "TutorialSceneTwo");
        multiNetworking.JoinRoom(settings);
    }

    public void CreateRoom()
    {

        s_roomName = i_roomName.text;
        if (s_roomName == "")
        {
            Debug.Log("RoomName can't be empty");
            return;
        }
        settings = new UnityMultiRoomSettings(RoomName: s_roomName, Password: "", SceneName: "TutorialSceneTwo");
        multiNetworking.CreateRoom(settings);
    }

    public void Connect()
    {
        s_username = i_username.text;
        multiNetworking.ConnectToServer(url, s_username);

        b_create.enabled = true;
        b_join.enabled = true;
        b_connect.enabled = false;
    }
}
