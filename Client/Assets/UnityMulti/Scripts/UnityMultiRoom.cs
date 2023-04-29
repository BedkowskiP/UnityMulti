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
    public string HostID { get; private set; }
    public string SceneName { get; private set; } = "";

    public UnityMultiRoomSettings(string RoomName)
    {
        this.RoomName = RoomName;
    }
    public UnityMultiRoomSettings(string RoomName, int MaxPlayers)
    {
        this.RoomName = RoomName;
        this.MaxPlayers = MaxPlayers;
    }
    public UnityMultiRoomSettings(string RoomName, string SceneName)
    {
        this.RoomName = RoomName;
        this.SceneName = SceneName;
    }
    public UnityMultiRoomSettings(string RoomName, string Password, int MaxPlayers)
    {
        this.RoomName = RoomName;
        this.Password = Password;
        this.MaxPlayers = MaxPlayers;
        this.IsPublic = false;
    }
    [JsonConstructor]
    public UnityMultiRoomSettings(string RoomName, string HostID, int MaxPlayers, bool IsPublic)
    {
        this.RoomName = RoomName;
        this.HostID = HostID;
        this.MaxPlayers = MaxPlayers;
        this.IsPublic = IsPublic;
    }

    public void SetHost(string host)
    {

    }
}

public class UnityMultiRoom : MonoBehaviour
{
    public UnityMultiRoomSettings Settings { get; private set; }
    public List<UnityMultiUser> UserList { get; private set; } = new List<UnityMultiUser>();
    private List<GameObject> roomObjectList = new List<GameObject>();
    private Dictionary<string, GameObject> clientObjectDict = new Dictionary<string, GameObject>();
    private UnityMultiNetworking multiNetworking;

    [JsonConstructor]
    public UnityMultiRoom(UnityMultiRoomSettings Settings, List<UnityMultiUser> UserList)
    {
        this.Settings = Settings;
        this.UserList = UserList;
    }

    public UnityMultiRoom(UnityMultiNetworking multiNetworking)
    {
        this.multiNetworking = multiNetworking;
    }

    public void AddUser(UnityMultiUser newUser)
    {
        UserList.Add(newUser);
        multiNetworking.ClientJoinM(newUser);
    }

    public void RemoveUser(UnityMultiUser user)
    {
        if (UserList.Contains(user))
        {
            UserList.Remove(user);
            multiNetworking.ClientLeaveM(user);
        }
    }
    public void SetSettings(UnityMultiRoomSettings settings)
    {
        this.Settings = settings;
    }

    public void SetNewHost(string host)
    {
        Settings.SetHost(host);
    }
}