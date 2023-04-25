using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMultiRoomSettings
{
    public string RoomName { get; private set; }
    public string Password { get; private set; } = "";
    public bool IsPublic { get; private set; } = true;
    public int MaxPlayers { get; private set; } = 10;

    public UnityMultiRoomSettings(string RoomName)
    {
        this.RoomName = RoomName;
    }
    public UnityMultiRoomSettings(string RoomName, int MaxPlayers)
    {
        this.RoomName = RoomName;
        this.MaxPlayers = MaxPlayers;
    }
    public UnityMultiRoomSettings(string RoomName, string Password)
    {
        this.RoomName = RoomName;
        this.Password = Password;
        this.IsPublic = false;
    }
    public UnityMultiRoomSettings(string RoomName, string Password, int MaxPlayers)
    {
        this.RoomName = RoomName;
        this.Password = Password;
        this.MaxPlayers = MaxPlayers;
        this.IsPublic = false;
    }
    [JsonConstructor]
    public UnityMultiRoomSettings(string RoomName, string Password, int MaxPlayers, bool IsPublic)
    {
        this.RoomName = RoomName;
        this.Password = Password;
        this.MaxPlayers = MaxPlayers;
        this.IsPublic = IsPublic;
    }
}

public class UnityMultiRoom : MonoBehaviour
{
    public UnityMultiRoomSettings Settings { get; private set; }
    private List<UnityMultiUser> UserList = new List<UnityMultiUser>();
    private List<GameObject> roomObjectList = new List<GameObject>();
    private Dictionary<string, GameObject> clientObjectDict = new Dictionary<string, GameObject>();
    private UnityMultiNetworking multiNetworking;
    private List<string> levels = new List<string>();

    [JsonConstructor]
    public UnityMultiRoom(UnityMultiRoomSettings Settings, List<UnityMultiUser> UserList)
    {
        this.Settings = Settings;
        this.UserList = UserList;
    }

    public UnityMultiRoom(UnityMultiRoomSettings Settings, UnityMultiNetworking multiNetworking)
    {
        this.Settings = Settings;
        this.multiNetworking = multiNetworking;
    }

    public void AddUser(UnityMultiUser newUser)
    {
        UserList.Add(newUser);
        multiNetworking.ClientJoinM(newUser);
    }

    public void removeUser(UnityMultiUser user)
    {
        UserList.Remove(user);
        multiNetworking.ClientLeaveM(user);
    }
}