using System.Collections;
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

    private void Awake()
    {
        b_create.interactable = false;
        b_join.interactable = false;
        b_connect.interactable = true;
    }
    private void Start()
    {
        StartCoroutine(ConnectionCor());
    }

    private IEnumerator ConnectionCor()
    {
        while (true)
        {
            if (multiNetworking.IsConnected)
            {
                isConnected.text = "Connected";
                isConnected.color = Color.green;
                b_create.interactable = true;
                b_join.interactable = true;
                b_connect.interactable = false;
            } else
            {
                isConnected.text = "Disconnected";
                isConnected.color = Color.red;
                b_create.interactable = false;
                b_join.interactable = false;
                b_connect.interactable = true;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void JoinRoom()
    {
        s_roomName = i_roomName.text;
        if (s_roomName == "")
        {
            Debug.Log("RoomName can't be empty");
            return;
        }
        multiNetworking.JoinRoom(new UnityMultiRoomSettings(RoomName: s_roomName, Password: ""));
    }

    public void CreateRoom()
    {

        s_roomName = i_roomName.text;
        if (s_roomName == "")
        {
            Debug.Log("RoomName can't be empty");
            return;
        }
        multiNetworking.CreateRoom(new UnityMultiRoomSettings(RoomName: s_roomName, Password: ""));
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

    private static System.Random random = new System.Random();
    public static string GenerateRandomUsername()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 10)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public override void OnJoinRoom(string roomName)
    {
        base.OnJoinRoom(roomName);
        multiNetworking.LoadLevel("TutorialSceneTwo");
    }
}
