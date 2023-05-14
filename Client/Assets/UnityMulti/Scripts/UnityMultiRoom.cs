using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnityMultiRoomSettings
{
    public string RoomName { get; private set; }
    public string Password { get; private set; } = "";
    public bool IsPublic { get; private set; } = true;
    public int MaxPlayers { get; private set; } = 10;
    public string HostID { get; private set; }
    public string SceneName { get; private set; } = "";

    public UnityMultiRoomSettings(string RoomName, string Password, string SceneName)
    {
        this.RoomName = RoomName;
        if (Password != "")
            IsPublic = false;
        this.Password = Password;
        this.SceneName = SceneName;
    }

    public UnityMultiRoomSettings(string RoomName, string Password, int MaxPlayers, string SceneName)
    {
        this.RoomName = RoomName;
        if (Password != "")
            IsPublic = false;
        this.Password = Password;
        this.MaxPlayers = MaxPlayers;
        this.SceneName = SceneName;
    }

    [JsonConstructor]
    public UnityMultiRoomSettings(string RoomName, string HostID, int MaxPlayers, bool IsPublic, string SceneName)
    {
        this.RoomName = RoomName;
        this.HostID = HostID;
        this.MaxPlayers = MaxPlayers;
        this.IsPublic = IsPublic;
        this.SceneName = SceneName;
    }

    public void SetHost(string HostID)
    {
        this.HostID = HostID;
    }

    public string GetSceneName()
    {
        return SceneName;
    }
}

public class UnityMultiRoom
{
    private UnityMultiNetworking multiNetworking;
    public UnityMultiRoomSettings Settings { get; private set; }
    public List<UnityMultiUser> UserList { get; private set; } = new List<UnityMultiUser>();

    public bool isSceneLoaded { get; private set; } = false;

    public void Reset()
    {
        Settings = null;
        UserList = new List<UnityMultiUser>();
    }

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

    public void SetNewHost(string HostID)
    {
        Settings.SetHost(HostID);
    }

    public void CreateRoom(UnityMultiRoomSettings settings)
    {
        if (!multiNetworking.isValidated)
        {
            multiNetworking.InvokeErrorCodes(ErrorCode.NotValidated);
            return;
        }
        else
        {
            Message roomSettings = new Message(MessageType.CREATE_ROOM, JsonConvert.SerializeObject(settings));
            multiNetworking.SendMessage(roomSettings);
        }
    }

    public void JoinRoom(UnityMultiRoomSettings settings)
    {
        if (!multiNetworking.isValidated)
        {
            multiNetworking.InvokeErrorCodes(ErrorCode.NotValidated);
            return;
        }
        else
        {
            Message roomSettings = new Message(MessageType.JOIN_ROOM, JsonConvert.SerializeObject(settings));
            multiNetworking.SendMessage(roomSettings);
        }
    }

    public void LeaveRoom()
    {
        if (!multiNetworking.isInRoom)
        {
            multiNetworking.InvokeErrorCodes(ErrorCode.NotInRoom);
            return;
        }
        else
        {
            Message leaveMessage = new Message(MessageType.LEAVE_ROOM, JsonConvert.SerializeObject(multiNetworking.room.Settings));
            multiNetworking.SendMessage(leaveMessage);
        }
    }

    public void ChangeHost(UnityMultiUser user)
    {
        Message newHost = new Message(MessageType.HOST_CHANGE, JsonConvert.SerializeObject(user.UserID));
        multiNetworking.SendMessage(newHost);
    }

    public void ChangeHost()
    {
        Message newHost = new Message(MessageType.HOST_CHANGE, "{\"UserID\": \"\"}");
        multiNetworking.SendMessage(newHost);
    }

    public void HandleCreateRoom(Message serverMessage)
    {
        if (serverMessage.ErrorCode != ErrorCode.None)
        {
            multiNetworking.InvokeErrorCodes(serverMessage.ErrorCode);
            return;
        }
        else
        {
            UnityMultiRoomSettings roomSettings = JsonConvert.DeserializeObject<UnityMultiRoomSettings>(serverMessage.Content);
            multiNetworking.InvokeRoomE("createRoom", roomSettings.RoomName);
        }
    }

    public async Task HandleJoinRoom(Message serverMessage)
    {
        if (serverMessage.ErrorCode != ErrorCode.None)
        {
            Debug.Log(serverMessage.ErrorCode);
            multiNetworking.InvokeErrorCodes(serverMessage.ErrorCode);
            return;
        }
        else
        {
            if (multiNetworking.isInRoom)
            {
                multiNetworking.InvokeErrorCodes(ErrorCode.AlreadyInRoom);
            }
            else
            {
                multiNetworking.SetInRoom(true);
                UnityMultiRoom placeholder = JsonConvert.DeserializeObject<UnityMultiRoom>(serverMessage.Content);
                SetSettings(placeholder.Settings);
                if (Settings.SceneName != "" && Settings.SceneName != null)
                {
                    try
                    {
                        isSceneLoaded = false;
                        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Settings.SceneName);
                        while (!asyncLoad.isDone)
                        {
                            await Task.Yield();
                        }
                        isSceneLoaded = true;      
                    } catch (Exception e)
                    {
                        Debug.Log(e);
                        Debug.Log("Unable to load scene: " + Settings.SceneName + "; Make sure the scene name is correct.");
                        LeaveRoom();
                        return;
                    }
                    if (isSceneLoaded)
                    {
                        multiNetworking.InvokeRoomE("joinRoom", Settings.SceneName);
                        foreach (var user in placeholder.UserList)
                        {
                            AddUser(user);
                        }
                    }
                } else { Debug.Log("SceneName is null or equal to \"\". Auto scene load function stopped."); return; }
            }
        }
    }
    public void HandleLeaveRoom()
    {
        multiNetworking.SetInRoom(false);
        multiNetworking.InvokeRoomE("leaveRoom", Settings.RoomName);
    }

    public void HandleUserLeave(Message serverMessage)
    {
        UnityMultiUser placeholder = JsonConvert.DeserializeObject<UnityMultiUser>(serverMessage.Content);
        RemoveUser(placeholder);
    }
    public void HandleUserJoin(Message serverMessage)
    {
        UnityMultiUser placeholder = JsonConvert.DeserializeObject<UnityMultiUser>(serverMessage.Content);
        AddUser(placeholder);
    }
    public void HandleHostChange(Message serverMessage)
    {
        UnityMultiUser placeholder = JsonConvert.DeserializeObject<UnityMultiUser>(serverMessage.Content);
        SetNewHost(placeholder.UserID);
    }
}