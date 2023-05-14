using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;
using System.Collections;
using UnityEditor;

[RequireComponent(typeof(UnityMainThreadDispatcher))]
public class UnityMultiNetworking : BaseSingleton<UnityMultiNetworking>, IDisposable
{
    protected override string GetSingletonName()
    {
        return "UnityMultiNetworking";
    }

    private WebSocket ws;
    public UnityMultiUser clientData { get; private set; }
    public UnityMultiRoom room { get; private set; }

    private volatile WebSocketState _webSocketState = WebSocketState.Connecting;
    public bool IsConnected => _webSocketState == WebSocketState.Open;
    //public bool IsConnected => ws?.ReadyState == WebSocketState.Open;

    public string connectionURL { get; private set; }
    public bool _autoReconnect = true;
    private bool _isReconnecting;
    public float ReconnectDelaySeconds = 10f;
    public int maxReconnectAttempt = 10;
    private int reconnectAttempt = 0;

    private long pingTimestamp;
    private long latency;
    public bool isConnectionReady { get; private set; } = false;
    public bool isDisconnecting { get; private set; } = false;
    public bool isValidated { get; private set; } = false;

    private bool isAppPlaying = false;

    public bool isInRoom { get; private set; } = false;

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

    private delegate void ReconnectE(CloseEventArgs close);
    private event ReconnectE ReconnectEvent;

    public delegate void RoomE(string roomName);
    public event RoomE CreateRoomEvent;
    public event RoomE JoinRoomEvent;
    public event RoomE LeaveRoomEvent;

    public delegate void RoomClientChangeE(UnityMultiUser user);
    public event RoomClientChangeE ClientJoinEvent;
    public event RoomClientChangeE ClientLeaveEvent;

    protected override void Awake() {
        room = new UnityMultiRoom(this);
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

    public void Connect(string url)
    {
        connectionURL = url;
        clientData = new UnityMultiUser();
        CreateConnection();
    }

    public void Connect(string url, string username)
    {
        connectionURL = url;
        clientData = new UnityMultiUser(username);
        CreateConnection();
    }

    private void CreateConnection()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            return;
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
                ClientDisconnectedEvent?.Invoke(close);
                ConnectionState();
                isConnectionReady = false;
                isValidated = false;
                room.Reset();
                ReconnectEvent?.Invoke(close);
                if (_autoReconnect && close.Code != 1000)
                {
                    StartCoroutine(Reconnect(close));
                }
            });
        };

        ws.Connect();
    }

    private void ConnectionState()
    {
        _webSocketState = ws.ReadyState;
        ConnectionStateChangeEvent?.Invoke(ws.ReadyState);
    }

    private IEnumerator Reconnect()
    {
        if (!_isReconnecting)
        {
            _isReconnecting = true;
            if (isAppPlaying)
            {
                    while (_isReconnecting && reconnectAttempt < maxReconnectAttempt)
                    {
                        if (IsConnected) break;
                        Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
                        CreateConnection();

                        yield return new WaitForSeconds(ReconnectDelaySeconds);
                        reconnectAttempt++;
                        if(!IsConnected) Dispose();
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
        }
        
    }


    private IEnumerator Reconnect(CloseEventArgs close)
    {
        if (!_isReconnecting && close.Code != 1000)
        {
            _isReconnecting = true;
            if (isAppPlaying)
            {
                while (_isReconnecting && reconnectAttempt < maxReconnectAttempt)
                {
                    if (IsConnected) break;
                    Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
                    Connect(ws.Url.ToString(), clientData.Username);

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
        }
    }

    public void Dispose()
    {
        if (ws != null)
        {
            ws.Close(1000, "Intentional disconnect");
            ws = null;
        }
    }

    public void Disconnect()
    {
        isDisconnecting = true;
        Dispose();
    }

    public void SendMessage(Message message)
    {
        if (IsConnected)
        {
            message.Timestamp = GetTimeNow();
            message.UserID = clientData.UserID;
            ws.Send(JsonConvert.SerializeObject(message));
        }
    }

    public long GetLatency()
    {
        if (latency.Equals(null))
            return 0;
        else
            return latency;
    }

    public void setTimeStamp(long pingTimestamp)
    {
        this.pingTimestamp = pingTimestamp;
    }

    private void RequestValidation()
    {
        Message validationRequest = new Message(MessageType.VALIDATION_REQUEST, JsonConvert.SerializeObject(clientData));

        SendMessage(validationRequest);
    }

    private void OnServerMessage(string message)
    {
            Message serverMessage = JsonConvert.DeserializeObject<Message>(message);
            switch (serverMessage.Type)
            {
                case MessageType.PONG:
                    HandlePong(serverMessage);
                    serverMessage = null;
                    break;
                case MessageType.VALIDATION_RESPONSE:
                    HandleValidation(serverMessage);
                    serverMessage = null;
                    break;
                case MessageType.CREATE_ROOM_RESPONSE:
                    room.HandleCreateRoom(serverMessage);
                    serverMessage = null;
                    break;
                case MessageType.JOIN_ROOM_RESPONSE:
                    room.HandleJoinRoom(serverMessage);
                    serverMessage = null;
                    break;
                case MessageType.USER_JOIN:
                    room.HandleUserJoin(serverMessage);
                    serverMessage = null;
                    break;
                case MessageType.USER_LEAVE:
                    room.HandleUserLeave(serverMessage);
                    serverMessage = null;
                    break;
                case MessageType.LEAVE_ROOM_RESPONSE:
                    room.HandleLeaveRoom();
                    break;
                case MessageType.HOST_CHANGE_RESPONSE:
                    room.HandleHostChange(serverMessage);
                    serverMessage = null;
                    break;
                case MessageType.ADD_UNITY_OBJECT_RESPONSE:
                    InstantiateNewObject(serverMessage);
                    serverMessage = null;
                    break;
                case MessageType.CHAT_MESSAGE:
                    // handle chat message
                    break;
                case MessageType.SERVER_MESSAGE:
                    // handle server message
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
    private void HandleValidation(Message serverMessage)
    {
        ValidationResult validationMessage = JsonConvert.DeserializeObject<ValidationResult>(serverMessage.Content);
        clientData.SetUserId(validationMessage.UserID);

        if (validationMessage.Validated)
        {
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

    public IEnumerator SendPing()
    {
        while (IsConnected)
        {
            Message pingMessage = new Message(MessageType.PING, GetTimeNow().ToString());

            SendMessage(pingMessage);

            setTimeStamp(GetTimeNow());
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private long GetTimeNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private void IsApplicationPlaying()
    {
        isAppPlaying = Application.isPlaying;
    }

    public bool IsHost()
    {
        if (room.Settings.HostID == clientData.UserID) return true;
        else return false;
    }

    public void SetInRoom(bool value)
    {
        isInRoom = value;
    }

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

    public void InstantiatePlayerObject(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale, GameObject parent)
    {
        if (!isValidated)
        {
            Debug.Log("User not validated yet. Can't instantiate objects while not validated.");
            return;
        }
        if (!isInRoom)
        {
            Debug.Log("User not in room. Can't instantiate objects while not in room.");
            return;
        }
        if (!room.isSceneLoaded)
        {
            Debug.Log("GameScene not loaded yet. Please wait before u try to instantiated objects");
            return;
        }

        GameObject tempObj = Resources.Load<GameObject>(prefabName);
        if (tempObj == null)
        {

            Debug.LogWarning("Couldn't load given prefab: " + prefabName);
            Resources.UnloadUnusedAssets();
            return;
        } else
        {
            
            UnityMultiObject mulitObjTemp = tempObj.GetComponent<UnityMultiObject>();
            if (mulitObjTemp == null)
            {
                Debug.LogWarning("Prefab '" + prefabName + "' don't have the <UnityMultiObject> component. Please add the component to the prefab before you try to instantiate it.");
            } else
            {
                
                UnityMultiObjectInfo temp = new UnityMultiObjectInfo(prefabName, position, rotation, scale, parent);
                Message objectMessage = new Message(MessageType.ADD_UNITY_OBJECT, JsonConvert.SerializeObject(temp));
                SendMessage(objectMessage);
            }
            Resources.UnloadUnusedAssets();
            return;
        }
    }

    private void InstantiateNewObject(Message serverMessage)
    {
        UnityMultiObjectInfo temp = JsonConvert.DeserializeObject<UnityMultiObjectInfo>(serverMessage.Content);

        GameObject tempObj = Resources.Load<GameObject>(temp.PrefabName);
        UnityMultiObject multiObject = tempObj.GetComponent<UnityMultiObject>();
        multiObject.SetParams(this, temp);

        try
        {
            GameObject parent = GameObject.Find(temp.ParentObject);
            if(parent == null) tempObj = Instantiate(tempObj, temp.Position.GetVec3(), temp.Rotation.GetQuat());
            else tempObj = Instantiate(tempObj, temp.Position.GetVec3(), temp.Rotation.GetQuat(), parent.transform);
            tempObj.name = "MultiObject("+temp.ObjectID+")";
        } catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}