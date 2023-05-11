using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;
using System.Collections;


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

    public bool IsConnected
    {
        get { return ws != null && ws.ReadyState == WebSocketState.Open; }
        private set { }
    }

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
        ws = new WebSocket(connectionURL);

        ws.OnOpen += (sender, args) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                InitialConnectionEvent?.Invoke();
                ConnectionStateChangeEvent?.Invoke(ws.ReadyState);
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
                ConnectionStateChangeEvent?.Invoke(ws.ReadyState);
            });
        };

        ws.OnClose += (sender, close) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                ClientDisconnectedEvent?.Invoke(close);
                ConnectionStateChangeEvent?.Invoke(ws.ReadyState);
                isConnectionReady = false;
                isValidated = false;
                ReconnectEvent?.Invoke(close);
                if (_autoReconnect)
                {
                    Reconnect(close);
                }
            });
        };

        ws.Connect();
    }

    public void Reconnect()
    {
        StartCoroutine(ReconnectCor());
    }
    public void Reconnect(CloseEventArgs close)
    {
        StartCoroutine(ReconnectCor(close));
    }

    private IEnumerator ReconnectCor()
    {
        if (!_isReconnecting)
        {
            if (isAppPlaying)
            {
                _isReconnecting = true;

                while (_isReconnecting && reconnectAttempt < maxReconnectAttempt && !IsConnected && isAppPlaying)
                {
                    Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
                    Connect(ws.Url.ToString(), clientData.Username);

                    yield return new WaitForSeconds(ReconnectDelaySeconds);
                    reconnectAttempt++;
                    Dispose();
                }

                if (reconnectAttempt >= maxReconnectAttempt)
                {
                    Debug.LogWarning("Reached max reconnect attempts: " + maxReconnectAttempt);
                }

                _isReconnecting = false;
                reconnectAttempt = 0;
            }
        }
    }


    private IEnumerator ReconnectCor(CloseEventArgs close)
    {
        if (!_isReconnecting && close.Code != 1000)
        {
            if (isAppPlaying)
            {
                _isReconnecting = true;

                while (_isReconnecting && reconnectAttempt < maxReconnectAttempt && !IsConnected && isAppPlaying)
                {
                    Debug.Log("Attempting to auto reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
                    Connect(ws.Url.ToString(), clientData.Username);

                    yield return new WaitForSeconds(ReconnectDelaySeconds);
                    reconnectAttempt++;
                    Dispose();
                }

                if (reconnectAttempt >= maxReconnectAttempt)
                {
                    Debug.LogWarning("Reached max reconnect attempts: " + maxReconnectAttempt);
                }

                _isReconnecting = false;
                reconnectAttempt = 0;
            }
        }
    }

    public void Dispose()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close(1000, "Intentional disconnect");
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
                    break;
                case MessageType.VALIDATION_RESPONSE:
                    HandleValidation(serverMessage);
                    break;
                case MessageType.CREATE_ROOM_RESPONSE:
                    room.HandleCreateRoom(serverMessage);
                    break;
                case MessageType.JOIN_ROOM_RESPONSE:
                    room.HandleJoinRoom(serverMessage);
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

    public void InstantiatePlayerObject(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale)
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

        try
        {
            UnityMultiObjectInfo temp = new UnityMultiObjectInfo(prefabName, position, rotation, scale);
            Message objectMessage = new Message(MessageType.ADD_UNITY_OBJECT, JsonConvert.SerializeObject(temp));
            SendMessage(objectMessage);
        } catch (Exception e)
        {
            Debug.LogError(e);
        }
        
    }
}