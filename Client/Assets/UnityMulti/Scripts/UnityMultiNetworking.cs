using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;

[RequireComponent(typeof(UnityMainThreadDispatcher))]
public class UnityMultiNetworking : BaseSingleton<UnityMultiNetworking>, IDisposable
{
    protected override string GetSingletonName()
    {
        return "UnityMultiNetworking";
    }

    private volatile WebSocket ws;
    private UnityMultiUser userData;
    private UnityMultiRoom room;

    public volatile WebSocketState _webSocketState = WebSocketState.Connecting;
    public bool IsConnected => _webSocketState == WebSocketState.Open;

    public string startingScene;

    public string connectionURL { get; private set; }
    public bool _autoReconnect = true;
    private bool _isReconnecting;
    public float ReconnectDelaySeconds = 2f;
    public int maxReconnectAttempt = 10;
    private int reconnectAttempt = 0;

    private long pingTimestamp;
    private long latency;
    public bool isConnectionReady { get; private set; } = false;
    public bool isDisconnecting { get; private set; } = false;
    public bool isValidated { get; private set; } = false;
    public bool isInRoom { get; private set; } = false;

    private bool isAppPlaying = false;

    public delegate void ServerMessageE(Message serverMessage);
    public event ServerMessageE CustomMessageEvent;

    public delegate void ErrorE(ErrorEventArgs error);
    public event ErrorE ClientErrorEvent;

    public delegate void ConnectedE();
    public event ConnectedE ClientConnectedAndReadyEvent;

    public delegate void DisconnectedE(CloseEventArgs close);
    public event DisconnectedE ClientDisconnectedEvent;

    public delegate void ConnectionStateE(WebSocketState state);
    public event ConnectionStateE ConnectionStateChangeEvent;

    public delegate void InitialConnectionE();
    public event InitialConnectionE InitialConnectionEvent;

    public delegate void MultiErrorE(ErrorCode errorCode);
    public event MultiErrorE MultiErrorEvent;

    public delegate void RoomE(string roomName);
    public event RoomE CreateRoomEvent;
    public event RoomE JoinRoomEvent;
    public event RoomE LeaveRoomEvent;

    public delegate void RoomClientChangeE(UnityMultiUser user);
    public event RoomClientChangeE ClientJoinEvent;
    public event RoomClientChangeE ClientLeaveEvent;

    #region setup
    protected override void Awake()
    {
        base.Awake();
        userData = gameObject.AddComponent<UnityMultiUser>();
        SetupRoom();
    }

    private void SetupRoom()
    {
        Destroy(room);
        room = gameObject.AddComponent<UnityMultiRoom>();
        room.AddNetworking(this);
        
    }

    private void ResetInstance()
    {
        isConnectionReady = false;
        isDisconnecting = false;
        isValidated = false;
        isInRoom = false;
        SetupRoom();
        if(gameObject.GetComponent<UnityMultiUser>() == null)
            userData = gameObject.AddComponent<UnityMultiUser>();
    }
    #endregion

    #region unityActions
    private void IsApplicationPlaying()
    {
        isAppPlaying = Application.isPlaying;
    }

    private void Update()
    {
        IsApplicationPlaying();
        if (!IsConnected) isInRoom = false;
        if (!isValidated && isInRoom) isInRoom = false;
    }

    private void OnDisable()
    {
        isAppPlaying = false;
        if (!isDisconnecting)
        {
            isDisconnecting = true;
            Disconnect();
        }
    }
    #endregion

    #region connection
    public void ConnectToServer(string url)
    {
        startingScene = SceneManager.GetActiveScene().name;
        connectionURL = url;
        CreateConnection();
    }

    public void ConnectToServer(string url, string username)
    {
        startingScene = SceneManager.GetActiveScene().name;
        connectionURL = url;
        userData.SetUsername(username);
        CreateConnection();
    }

    private void CreateConnection()
    {
        isValidated = false;
        isConnectionReady = false;
        if (room != null)
        {
            ResetInstance();
        }

        if (ws != null)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                return;
            }
            else
            {
                ws.CloseAsync();
            }
        }     

        ws = new WebSocket(connectionURL);

        ws.OnOpen += (sender, args) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                InitialConnectionEvent?.Invoke();
                ConnectionState();
                RequestValidation();
            });
        };

        ws.OnMessage += (sender, message) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                OnServerMessage(message.Data);
            });
        };

        ws.OnError += (sender, error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                ClientErrorEvent?.Invoke(error);
                ConnectionState();
            });
        };

        ws.OnClose += (sender, close) =>
        {
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                ConnectionState();
                StopCoroutine(SendPing());
                ClientDisconnectedEvent?.Invoke(close);
                ws.Close();
                ws = null;
                if (_autoReconnect && close.Code != 1000)
                {
                    
                    //CreateConnection();
                    //StartCoroutine(Reconnect(close));
                    StartCoroutine(ReconnectCor());
                }            
                
            });
        };

        ws.Connect();
    }   

    public void Dispose()
    {
        if (ws != null)
        {
            try
            {
                ws.Close(1000, "Intentional disconnect");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the close operation.
                Debug.LogError($"Error closing WebSocket: {ex.Message}");
            }
            finally
            {
                
            }
        }
    }

    public void Disconnect()
    {
        isDisconnecting = true;
        Dispose();
    }

    private void ConnectionState()
    {
        if(ws != null)
        {
            _webSocketState = ws.ReadyState;
            ConnectionStateChangeEvent?.Invoke(ws.ReadyState);
        }
    }

    public void Reconnect()
    {
        StartCoroutine(ReconnectCor());
    }

    private IEnumerator ReconnectCor()
    {
        if (!_isReconnecting)
        {
            _isReconnecting = true;
            if (isAppPlaying)
            {
                Debug.Log("Starting reconnecting");
                while (_isReconnecting && reconnectAttempt < maxReconnectAttempt)
                {
                    if (IsConnected)
                    {
                        break;
                    }

                    Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
                    CreateConnection();

                    yield return new WaitForSeconds(ReconnectDelaySeconds);
                    reconnectAttempt++;
                    if (!IsConnected) Dispose();
                }

                if (IsConnected)
                {
                    Debug.Log("Reconnected successfully");
                }
                else if (reconnectAttempt >= maxReconnectAttempt)
                {
                    Debug.LogWarning("Reached max reconnect attempts: " + maxReconnectAttempt);
                }

                reconnectAttempt = 0;
            }
            _isReconnecting = false;
        }
        
    }
    #endregion

    #region serverActions

    private async void OnServerMessage(string message)
    {
            Message serverMessage = JsonConvert.DeserializeObject<Message>(message, CustomConverters.settings);
            switch (serverMessage.Type)
            {
                case MessageType.PONG:
                    HandlePong(serverMessage);
                    break;
                case MessageType.VALIDATION_RESPONSE:
                    HandleValidation(serverMessage);
                    break;
                case MessageType.CREATE_ROOM_RESPONSE:
                    room.HandleCreateRoom(serverMessage);
                    break;
                case MessageType.JOIN_ROOM_RESPONSE:
                    await room.HandleJoinRoom(serverMessage);
                    break;
                case MessageType.USER_JOIN:
                    room.HandleUserJoin(serverMessage);
                    break;
                case MessageType.USER_LEAVE:
                    room.HandleUserLeave(serverMessage);
                    break;
                case MessageType.LEAVE_ROOM_RESPONSE:
                    room.HandleLeaveRoom();
                    break;
                case MessageType.HOST_CHANGE_RESPONSE:
                    room.HandleHostChange(serverMessage);
                    break;
                case MessageType.ADD_UNITY_OBJECT_RESPONSE:
                    InstantiateNewObject(serverMessage);
                    break;
                case MessageType.TRANSFORM_UPDATE_RESPONSE:
                    UpdateObjectTransform(serverMessage);
                    break;
                case MessageType.RPC_METHOD_RESPONSE:
                    HandleRPC(serverMessage);
                    break;
                default:
                    if (MessageType.CUSTOM.Contains(serverMessage.Type))
                    {
                        CustomMessageEvent?.Invoke(serverMessage);
                    }
                    else
                    {
                        Debug.Log("Unknown message type: " + serverMessage.Type);
                    }
                    break;
            }
            
    }

    private void setTimeStamp(long pingTimestamp)
    {
        this.pingTimestamp = pingTimestamp;
    }

    private void RequestValidation()
    {
        Message validationRequest = new Message(MessageType.VALIDATION_REQUEST, userData.Serialize(new UnityMultiUserHelper(userData)));

        SendMessage(validationRequest);
    }

    private void HandleValidation(Message serverMessage)
    {
        if (serverMessage.ErrorCode == 0)
        {
            UnityMultiUser validationMessage = new UnityMultiUser();
            validationMessage.Deserialize(serverMessage.Content);
            userData.SetUserId(validationMessage.UserID);
            isValidated = true;

            ClientConnectedAndReadyEvent?.Invoke();
            StartCoroutine(SendPing());
        }
        else
        {
            MultiErrorEvent?.Invoke(serverMessage.ErrorCode);
            Disconnect();
        }
    }

    private void HandlePong(Message serverMessage)
    {
        latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pingTimestamp;
    }

    private IEnumerator SendPing()
    {
        while (IsConnected)
        {
            Message pingMessage = new Message(MessageType.PING, GetTimeNow().ToString());

            SendMessage(pingMessage);

            setTimeStamp(GetTimeNow());
            yield return new WaitForSecondsRealtime(5f);
        }
    }

    private long GetTimeNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public async void InstantiatePlayerObject(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale, UnityMultiUser user)
    {
        
        while (!isValidated)
        {
            await Task.Yield();
        }

        if (!isInRoom)
        {
            Debug.Log("User not in room. Can't instantiate objects while not in room.");
            return;
        }

        while (!room.isSceneLoaded)
        {
            await Task.Yield();
        }

        
        GameObject tempObj = Resources.Load<GameObject>(prefabName);
        if (tempObj == null)
        {

            Debug.LogWarning("Couldn't load given prefab: " + prefabName);
            Resources.UnloadUnusedAssets();
            return;
        }
        else
        {

            UnityMultiObject mulitObjTemp = tempObj.GetComponent<UnityMultiObject>();
            if (mulitObjTemp == null)
            {
                Debug.LogWarning("Prefab '" + prefabName + "' don't have the <UnityMultiObject> component. Please add the component to the prefab before you try to instantiate it.");
            }
            else
            {

                UnityMultiObjectInfo temp = new UnityMultiObjectInfo(prefabName, position, rotation, scale, user.UserID);
                Message objectMessage = new Message(MessageType.ADD_UNITY_OBJECT, JsonConvert.SerializeObject(temp, CustomConverters.settings));
                SendMessage(objectMessage);
            }
            Resources.UnloadUnusedAssets();
            return;
        }
    }

    private async void InstantiateNewObject(Message serverMessage)
    {
        while (!room.isSceneLoaded)
        {
            await Task.Yield();
        }

        UnityMultiObjectInfo temp = JsonConvert.DeserializeObject<UnityMultiObjectInfo>(serverMessage.Content, CustomConverters.settings);

        GameObject loadedObj = Resources.Load<GameObject>(temp.PrefabName);
        GameObject createdObj = null;
        UnityMultiObject multiObject = loadedObj.GetComponent<UnityMultiObject>();
        multiObject.SetParams(this, temp);

        try
        {
            GameObject parent = room.GetUserObjByID(temp.Owner);
            if (parent == null) createdObj = Instantiate(loadedObj, temp.Position, temp.Rotation);
            else createdObj = Instantiate(loadedObj, temp.Position, temp.Rotation, parent.transform);
            createdObj.name = "MultiObject(" + temp.ObjectID + ")";
            if (parent != null) parent.GetComponent<UnityMultiUser>().UserObjectList.Add(createdObj);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async void UpdateObjectTransform(Message serverMessage)
    {
        while (!room.isSceneLoaded)
        {
            await Task.Yield();
        }

        UnityMultiTransformInfo transformUpdate = JsonConvert.DeserializeObject<UnityMultiTransformInfo>(serverMessage.Content, CustomConverters.settings);
        GameObject temp = GameObject.Find(transformUpdate.ObjName);
        if(temp == null)
        {
            Debug.Log("Couldn't find object to update.");
            return;
        }
        temp.GetComponent<UnityMultiObjectTransform>().UpdateTransform(transformUpdate);
    }

    public void RPC(GameObject gameObject, string methodName, object[] parameters, RPCTarget target)
    {
        MethodInfo myMethod = GetMethodInfo(gameObject, methodName);

        if(myMethod == null)
        {
            Debug.Log("Couldn't find given method: '" + methodName + "'.");
            return;
        }

        Dictionary<string, object> jsonObject = new Dictionary<string, object>();
        jsonObject["parameters"] = parameters;

        UnityMultiRPCInfo newRPC = new UnityMultiRPCInfo(methodName, parameters, gameObject.name, target);
        Message newRPCMessage = new Message(MessageType.RPC_METHOD, JsonConvert.SerializeObject(newRPC, CustomConverters.settings));
        SendMessage(newRPCMessage);
    }

    private async void HandleRPC(Message serverMessage)
    {
        while (!room.isSceneLoaded)
        {
            await Task.Yield();
        }

        UnityMultiRPCInfo rpc = JsonConvert.DeserializeObject<UnityMultiRPCInfo>(serverMessage.Content, CustomConverters.settings);
        GameObject obj = GameObject.Find(rpc.ObjName);

        if(obj == null)
        {
            Debug.Log("Couldn't find object with given name: '" + rpc.ObjName + "'.");
            return;
        }

        Component[] components = obj.GetComponents<Component>();

        foreach (Component component in components)
        {
            Type type = component.GetType();

            MethodInfo methodToRun = type.GetMethod(rpc.MethodName);

            if (methodToRun != null)
            {
                if (methodToRun.GetCustomAttributes(typeof(UnityMultiRPC), true).Length > 0)
                {
                    ParameterInfo[] methodParameters = methodToRun.GetParameters();
                    object[] convertedParameters = new object[methodParameters.Length];

                    for (int i = 0; i < methodParameters.Length; i++)
                    {
                        Type parameterType = methodParameters[i].ParameterType;

                        if (parameterType == typeof(Color))
                        {
                            Color colorParameter = JsonConvert.DeserializeObject<Color>(rpc.Parameters[i].ToString(), CustomConverters.settings);
                            convertedParameters[i] = colorParameter;
                        }
                        else if (parameterType == typeof(Vector3))
                        {
                            Vector3 vectorParameter = JsonConvert.DeserializeObject<Vector3>(rpc.Parameters[i].ToString(), CustomConverters.settings);
                            convertedParameters[i] = vectorParameter;
                        }
                        else if (parameterType == typeof(Quaternion))
                        {
                            Quaternion vectorParameter = JsonConvert.DeserializeObject<Quaternion>(rpc.Parameters[i].ToString(), CustomConverters.settings);
                            convertedParameters[i] = vectorParameter;
                        }
                        else
                        {
                            convertedParameters[i] = rpc.Parameters[i];
                        }
                    }

                    methodToRun.Invoke(component, convertedParameters);
                    return;
                }
                else
                {
                    Debug.Log(methodToRun.Name + " doesn't own [UnityMultiRPC] attribute.");
                    return;
                }
            }
        }

        Debug.Log(obj.name + " doesn't own method of name '" + rpc.MethodName + "'.");
    }

    #endregion

    #region userActions

    public void LoadLevel()
    {
        Debug.Log("Loading new level...");
        foreach (GameObject obj in room.multiUserList)
        {
            Destroy(obj);
        }
        userData.UserObjectList = new List<GameObject>();
        SetupRoom();
        SceneManager.LoadSceneAsync(startingScene, LoadSceneMode.Single);
    }

    public void SendMessage(Message message)
    {
        if (IsConnected)
        {
            message.Timestamp = GetTimeNow();
            message.UserID = userData.UserID;
            ws.Send(JsonConvert.SerializeObject(message, CustomConverters.settings));
        }
    }

    public long GetLatency()
    {
        if (latency.Equals(null))
            return 0;
        else
            return latency;
    }

    public bool IsHost()
    {
        if(!isInRoom)
        {
            Debug.Log("User not in room.");
            return false;
        } else if (room.Settings.HostID == userData.UserID) return true;
        else return false;
    }

    public void CreateRoom(UnityMultiRoomSettings settings)
    {
        room.CreateRoom(settings);
    }

    public void JoinRoom(UnityMultiRoomSettings settings)
    {
        room.JoinRoom(settings);
    }

    public void LeaveRoom()
    {
        room.LeaveRoom();
    }

    public string GetUserID()
    {
        return userData.UserID;
    }

    public void ChangeHost(UnityMultiUser user)
    {
        if(!isInRoom)
        {
            Debug.Log("Can't change host while not in room");
            return;
        }
        room.ChangeHost(user);
    }

    public void ChangeHost()
    {
        if (!isInRoom)
        {
            Debug.Log("Can't change host while not in room");
            return;
        }
        room.ChangeHost();
    }

    #endregion

    public void SetInRoom(bool value)
    {
        isInRoom = value;
    }

    private MethodInfo GetMethodInfo(GameObject gameObject, string methodName)
    {
        Component[] components = gameObject.GetComponents<Component>();

        foreach (Component component in components)
        {
            Type type = component.GetType();
            MethodInfo methodInfo = type.GetMethod(methodName);

            if (methodInfo != null)
            {
                return methodInfo;
            }
        }

        return null;
    }

    #region eventActions
    public void InvokeErrorCodes(ErrorCode code)
    {
        MultiErrorEvent?.Invoke(code);
    }

    public void InvokeRoomE(string value, string roomName)
    {
        switch (value)
        {
            case "createRoom":
                CreateRoomEvent?.Invoke(roomName);
                break;
            case "joinRoom":
                JoinRoomEvent?.Invoke(roomName);
                break;
            case "leaveRoom":
                LeaveRoomEvent?.Invoke(roomName);
                SetupRoom();
                break;
            default:
                break;
        }
    }

    public void ClientJoinM(UnityMultiUser user)
    {
        ClientJoinEvent?.Invoke(user);
    }

    public void ClientLeaveM(UnityMultiUser user)
    {
        ClientLeaveEvent?.Invoke(user);
    }

    #endregion
}

