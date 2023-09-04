using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class CreateConnection : UnityMultiNetworkingCallbacks
{

    public string url = "ws://localhost:8080";

    public Button b_create;
    public Button b_join;
    public Button b_connect;
    public Button b_disconnect;

    public InputField i_ip;
    public InputField i_roomName;
    public InputField i_username;

    private string s_roomName;
    private string s_username;

    public Text isConnected;

    private void Awake()
    {
        b_join.interactable = false;
        b_create.interactable = false;
        b_disconnect.interactable = false;
        b_connect.interactable = true;
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
        url = i_ip.text;
        s_username = i_username.text;
        multiNetworking.ConnectToServer(url, s_username);

        b_create.interactable = true;
        b_join.interactable = true;
        b_connect.interactable = false;
        b_disconnect.interactable = true;
    }

    public void Disconnect()
    {
        multiNetworking.Disconnect();
    }

    public override void OnClientDisconnected(CloseEventArgs close)
    {
        base.OnClientDisconnected(close);
        Debug.Log("dis");
    }

    private IEnumerator ConnectedCor()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (multiNetworking.IsConnected)
            {
                isConnected.text = "Connected";
                isConnected.color = Color.green;
                b_disconnect.interactable = true;
                b_connect.interactable = false;
                b_join.interactable = true;
                b_create.interactable = true;
            }
            else
            {
                isConnected.text = "Disconnected";
                isConnected.color = Color.red;
                b_disconnect.interactable = false;
                b_connect.interactable = true;
                b_join.interactable = false;
                b_create.interactable = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Start()
    {
        StartCoroutine(ConnectedCor());
    }
}
