using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
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

    public UnityMultiRoomSettings(string RoomName, string Password)
    {
        this.RoomName = RoomName;
        if (Password != "")
            IsPublic = false;
        this.Password = Password;
    }

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

public class UnityMultiRoom : UnityMultiSerializer<UnityMultiRoomHelper>
{
    private UnityMultiNetworking multiNetworking;
    public UnityMultiRoomSettings Settings { get; private set; }

    public List<GameObject> multiUserList = new List<GameObject>();

    public bool isSceneLoaded { get; private set; } = false;

    public override void Deserialize(string obj)
    {
        base.Deserialize(obj);
    }

    public void Reset()
    {
        Settings = null;
        if(multiUserList.Count > 0)
        {
            for (int i = multiUserList.Count - 1; i >= 0; i--)
            {
                GameObject obj = multiUserList[i];
                if (obj != null)
                {
                    Destroy(obj);
                }
                multiUserList.RemoveAt(i);
            }
        }
        multiUserList = new List<GameObject>();
    }  

    public void AddNetworking(UnityMultiNetworking multiNetworking)
    {
        this.multiNetworking = multiNetworking;
    }

    public UnityMultiUser FindUserByUserName(string username)
    {
        if (multiUserList.Count > 0)
        {
            for (int i = 0; i < multiUserList.Count; i++)
            {
                if (multiUserList[i].GetComponent<UnityMultiUser>().Username == username)
                    return multiUserList[i].GetComponent<UnityMultiUser>();
            }
        }

        return null;
    }

    public UnityMultiUser FindUserById(string userID)
    {
        if (multiUserList.Count > 0)
        {
            for (int i = 0; i < multiUserList.Count; i++)
            {
                if (multiUserList[i].GetComponent<UnityMultiUser>().UserID == userID)
                    return multiUserList[i].GetComponent<UnityMultiUser>();
            }
        }

        return null;
    }

    public GameObject GetUserObjByID(string userID)
    {
        for (int i = multiUserList.Count - 1; i >= 0; i--)
        {
            GameObject obj = multiUserList[i];
            if (obj.GetComponent<UnityMultiUser>().UserID == userID)
            {
                return multiUserList[i];
            }
        }
        return null;
    }

    public void AddUser(UnityMultiUser newUser)
    {
        GameObject newUserGO = new GameObject("EmptyObject");
        newUserGO.name = newUser.Username;
        newUserGO.transform.parent = this.transform;
        newUserGO.AddComponent<UnityMultiUser>().SetParams(newUser);

        if (!multiUserList.Contains(newUserGO))
        {
            multiUserList.Add(newUserGO);
            multiNetworking.ClientJoinM(newUser);
        }
        
    }

    public void RemoveUser(UnityMultiUser userToRemove)
    {
        UnityMultiUser userComp;

        for(int i = multiUserList.Count -1; i >= 0; i--)
        {
            userComp = multiUserList[i].GetComponent<UnityMultiUser>();

            if(userToRemove.UserID == userComp.UserID)
            {
                for (int j = userComp.UserObjectList.Count - 1; j >= 0; j--)
                {
                    GameObject obj = userComp.UserObjectList[j];
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
                Destroy(multiUserList[i]);
                multiUserList.Remove(multiUserList[i]);
            }
        }
        multiNetworking.ClientLeaveM(userToRemove);
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
            Message leaveMessage = new Message(MessageType.LEAVE_ROOM, JsonConvert.SerializeObject(Settings));
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
                UnityMultiRoomHelper placeholder = JsonConvert.DeserializeObject<UnityMultiRoomHelper>(serverMessage.Content);
                SetSettings(placeholder.Settings);
                if (Settings.SceneName != "" && Settings.SceneName != null)
                {
                    if(SceneManager.GetActiveScene().name != Settings.SceneName)
                    {
                        try
                        {
                            isSceneLoaded = false;
                            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Settings.SceneName, LoadSceneMode.Single);
                            while (!asyncLoad.isDone)
                            {
                                await Task.Yield();
                            }
                            isSceneLoaded = true;
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                            Debug.Log("Unable to load scene: " + Settings.SceneName + "; Make sure the scene name is correct.");
                            LeaveRoom();
                            return;
                        }
                        
                    } else { isSceneLoaded = true; }

                    if (isSceneLoaded)
                    {
                        multiNetworking.InvokeRoomE("joinRoom", Settings.SceneName);
                        bool ignore;
                        foreach (var user in placeholder.multiUserList)
                        {
                            ignore = false;
                            foreach(var userOnList in multiUserList)
                            {
                                if (user == userOnList.GetComponent<UnityMultiUser>()) { ignore = true; break; }
                            }
                            if(!ignore) AddUser(user);
                        }
                    }
                } else { Debug.Log("SceneName is null or equal to \"\". Auto scene load function stopped."); return; }
            }
        }
        return;
    }
    public void HandleLeaveRoom()
    {
        multiNetworking.SetInRoom(false);

        multiNetworking.InvokeRoomE("leaveRoom", Settings.RoomName);
    }

    public void HandleUserLeave(Message serverMessage)
    {
        UnityMultiUser placeholder = new UnityMultiUser();
        placeholder.Deserialize(serverMessage.Content);
        RemoveUser(placeholder);
    }
    public void HandleUserJoin(Message serverMessage)
    {
        UnityMultiUser placeholder = new UnityMultiUser();
        placeholder.Deserialize(serverMessage.Content);
        AddUser(placeholder);
    }
    public void HandleHostChange(Message serverMessage)
    {
        UnityMultiUser placeholder = new UnityMultiUser();
        placeholder.Deserialize(serverMessage.Content);
        SetNewHost(placeholder.UserID);
    }

}

public class UnityMultiRoomHelper
{
    public UnityMultiRoomSettings Settings { get; private set; }

    public List<UnityMultiUser> multiUserList = new List<UnityMultiUser>();
    [SerializeField]
    public List<GameObject> loadedPrefabs = new List<GameObject>();

    [JsonConstructor]
    public UnityMultiRoomHelper(UnityMultiRoomSettings Settings, List<UnityMultiUser> UserList)
    {
        this.Settings = Settings;
        this.multiUserList = UserList;
    }

    public UnityMultiRoomHelper()
    {

    }
}