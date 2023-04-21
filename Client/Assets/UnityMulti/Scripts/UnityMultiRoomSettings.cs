using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMultiRoomSettings
{
    public string RoomName { get; private set; }
    public string RoomID { get; private set; }
    public string Password { get; private set; }
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
}

public class UnityMultiRoom : MonoBehaviour
{
    public UnityMultiRoomSettings settings { get; private set; }
    private List<UnityMultiUser> clientList = new List<UnityMultiUser>();
    private List<GameObject> roomObjectList = new List<GameObject>();
    private Dictionary<string, GameObject> clientObjectDict = new Dictionary<string, GameObject>();
    private UnityMultiNetworking multiNetworking;

    public UnityMultiRoom(UnityMultiRoomSettings settings, UnityMultiNetworking multiNetworking)
    {
        this.settings = settings;
        this.multiNetworking = multiNetworking;
    }

    public void AddUser(UnityMultiUser newUser)
    {
        clientList.Add(newUser);
        multiNetworking.ClientJoinM(newUser);
    }

    public void removeUser(UnityMultiUser user)
    {
        clientList.Remove(user);
        multiNetworking.ClientLeaveM(user);
    }
}