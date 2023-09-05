using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class CreateConnection : UnityMultiNetworkingCallbacks
{

    public string url = "ws://localhost:8080";

    public Button b_create;
    public Button b_join;
    public Button b_connect;

    public InputField i_ip;
    public InputField i_roomName;
    public InputField i_username;

    private string s_roomName;
    private string s_username;

    public Text isConnected;

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
        if (i_ip.text == null || i_ip.text == "")
            url = "ws://localhost:8080";
        else url = i_ip.text;
        if (i_username.text == null || i_username.text == "")
            s_username = GenerateRandomUsername();
        else s_username = i_username.text;
        multiNetworking.ConnectToServer(url, s_username);
    }

    public override void OnClientConnected()
    {
        base.OnClientConnected();
        if (isConnected != null)
        {
            isConnected.text = "Connected";
            isConnected.color = Color.green;
        }
        else
        {
            Debug.LogWarning("The isConnected Text component is null or has been destroyed.");
        }
    }

    private static System.Random random = new System.Random();
    public static string GenerateRandomUsername()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 10)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
