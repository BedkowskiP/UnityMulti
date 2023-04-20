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
    public string RoomID { get; private set; }

    public List<UnityMultiUser> clientList = new List<UnityMultiUser>();

    //mozliwe ze tutaj jakis player list itd sie jeszcze doda
}