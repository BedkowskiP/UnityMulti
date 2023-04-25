using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(UnityMainThreadDispatcher))]
public class UnityMultiNetworking : BaseSingleton<UnityMultiNetworking>, IDisposable
{
    protected override string GetSingletonName()
    {
        return "UnityMultiNetworking";
    }

    private WebSocket ws;
    public UnityMultiUser clientData { get; private set; }
    public UnityMultiRoom roomData { get; private set; }

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

    private bool isInRoom = false;

    public delegate void ServerMessageE(Message serverMessage);
    public event ServerMessageE CustomMessageEvent;

    public delegate void ErrorE(ErrorEventArgs error);
    public event ErrorE ClientErrorEvent;

    public delegate void ConnectedE();
    public event ConnectedE ClientConnectedAndReadyEvent;

    public delegate void DisconnectedE(CloseEventArgs close);
    public event DisconnectedE ClientDisconnectedEvent;

    public delegate void ConnectionStateE();
    public event ConnectionStateE ConnectionStateChangeEvent;

    public delegate void InitialConnectionE();
    public event InitialConnectionE InitialConnectionEvent;

    public delegate void MultiErrorE(ErrorCode errorCode);
    public event MultiErrorE MultiErrorEvent;

    private delegate void ReconnectE(CloseEventArgs close);
    private event ReconnectE ReconnectEvent;

    public delegate void JoinRoomE(string roomName);
    public event JoinRoomE JoinRoomEvent;

    public delegate void CreateRoomE(string roomName);
    public event CreateRoomE CreateRoomEvent;

    public delegate void RoomClientChangeE(UnityMultiUser user);
    public event RoomClientChangeE ClientJoinEvent;
    public event RoomClientChangeE ClientLeaveEvent;

    protected override void Awake()
    {
        base.Awake();
        ReconnectEvent += Reconnect;
    }

    private void Update()
    {
        IsApplicationPlaying();
    }
    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        isAppPlaying = false;
        if (!isDisconnecting)
        {
            isDisconnecting = true;
            Disconnect();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    public void Connect(string url)
    {
        connectionURL = url;
        clientData = new UnityMultiUser();
        CreateConnection();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="username"></param>
    public void Connect(string url, string username)
    {
        connectionURL = url;
        clientData = new UnityMultiUser(username);
        CreateConnection();
    }

    /// <summary>
    /// 
    /// </summary>
    private void CreateConnection()
    {
        ws = new WebSocket(connectionURL);

        ws.OnOpen += (sender, args) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                InitialConnectionEvent?.Invoke();
                ConnectionStateChangeEvent?.Invoke();
                RequestValidation();
            });
        };

        ws.OnMessage += (sender, message) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnServerMessage(message.Data));
        };

        ws.OnError += (sender, error) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                ClientErrorEvent?.Invoke(error);
                ConnectionStateChangeEvent?.Invoke();
            });
        };

        ws.OnClose += (sender, close) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                ClientDisconnectedEvent?.Invoke(close);
                ConnectionStateChangeEvent?.Invoke();
            });
            isConnectionReady = false;
            isValidated = false;
            ReconnectEvent?.Invoke(close);
        };

        ws.Connect();
    }

    /// <summary>
    /// Auto reconnect method
    /// </summary>
    private void Reconnect(CloseEventArgs close)
    {
        if (!_isReconnecting && close.Code != 1000)
        {
            if (_autoReconnect)
            {

                if (isAppPlaying)
                {
                    _isReconnecting = true;

                    while (_isReconnecting && reconnectAttempt < maxReconnectAttempt && !IsConnected && isAppPlaying)
                    {
                        Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
                        Connect(ws.Url.ToString(), clientData.Username);

                        new WaitForSeconds(ReconnectDelaySeconds);
                        reconnectAttempt++;
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
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Close(1000, "Intentional disconnect");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Disconnect()
    {
        isDisconnecting = true;
        Dispose();
    }

    /// <summary>
    /// Send any string message to server
    /// </summary>
    /// <param name="message"></param>
    public new void SendMessage(string message)
    {
        if (IsConnected)
        {
            ws.Send(message);
        }
    }

    /// <summary>
    /// Variant of send message which send message to server in form of JSON
    /// </summary>
    /// <param name="message"></param>
    public void SendMessage(Message message)
    {
        if (IsConnected)
        {
            ws.Send(JsonConvert.SerializeObject(message));
        }
    }

    /// <summary>
    /// return current websocket state
    /// </summary>
    /// <returns></returns>
    public WebSocketState getState()
    {
        return ws.ReadyState;
    }

    /// <summary>
    /// return latency
    /// </summary>
    /// <returns></returns>
    public long GetLatency()
    {
        if (latency.Equals(null))
            return 0;
        else
            return latency;
    }

    /// <summary>
    /// set timestamp of last ping
    /// </summary>
    /// <param name="pingTimestamp"></param>
    public void setTimeStamp(long pingTimestamp)
    {
        this.pingTimestamp = pingTimestamp;
    }

    /// <summary>
    /// send message with validation request to server
    /// </summary>
    private void RequestValidation()
    {
        Message validationRequest = new Message(MessageType.VALIDATION_REQUEST, JsonConvert.SerializeObject(clientData), GetTimeNow(), clientData.UserID );

        SendMessage(validationRequest);
    }

    public void CreateRoom(UnityMultiRoomSettings settings)
    {
        if(!isValidated)
        {
            MultiErrorEvent?.Invoke(ErrorCode.NotValidated);
        }
        else 
        {
            Message roomSettings = new Message(MessageType.CREATE_ROOM, JsonConvert.SerializeObject(settings), GetTimeNow(), clientData.UserID);
            SendMessage(roomSettings);
        }
    }

    public void JoinRoom(UnityMultiRoomSettings settings)
    {
        if (!isValidated)
        {
            MultiErrorEvent?.Invoke(ErrorCode.NotValidated);
        }
        else
        {
            Message roomSettings = new Message(MessageType.JOIN_ROOM, JsonConvert.SerializeObject(settings), GetTimeNow(), clientData.UserID);
            SendMessage(roomSettings);
        }
    }


    /// <summary>
    /// Base server message handler
    /// </summary>
    /// <param name="message"></param>
    private void OnServerMessage(string message)
    {
        try
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
                    HandleCreateRoom(serverMessage);
                    break;
                case MessageType.JOIN_ROOM_RESPONSE:
                    HandleJoinRoom(serverMessage);
                    break;
                case MessageType.PLAYER_POSITION:
                    // handle player position message
                    break;
                case MessageType.PLAYER_ROTATION:
                    // handle player rotation message
                    break;
                case MessageType.PLAYER_SCALE:
                    // handle player scale message
                    break;
                case MessageType.SERVER_STATUS:
                    // handle server status message
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
        catch (Exception e)
        {
            Debug.LogError("Recevied message error: " + e.Message + " \nMessage from server: " + message);
        }
    }

    /// <summary>
    /// method which set latency based on server message response time
    /// </summary>
    /// <param name="serverMessage"></param>
    private void HandlePong(Message serverMessage)
    {
        latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pingTimestamp;
    }

    /// <summary>
    /// validation handler
    /// </summary>
    /// <param name="serverMessage"></param>
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

    private void HandleCreateRoom(Message serverMessage)
    {
        if(serverMessage.ErrorCode != ErrorCode.None)
        {
            MultiErrorEvent?.Invoke(serverMessage.ErrorCode);
        } 
        else
        {
            UnityMultiRoomSettings roomSettings = JsonConvert.DeserializeObject<UnityMultiRoomSettings>(serverMessage.Content);
            CreateRoomEvent?.Invoke(roomSettings.RoomName);
        }
    }
    private void HandleJoinRoom(Message serverMessage)
    {
        if (serverMessage.ErrorCode != ErrorCode.None)
        {
            Debug.Log(serverMessage.ErrorCode);
            MultiErrorEvent?.Invoke(serverMessage.ErrorCode);
        }
        else
        {
            if (isInRoom)
            {
                MultiErrorEvent?.Invoke(ErrorCode.AlreadyInRoom);
            } 
            else
            {
                UnityMultiRoom placeholder = JsonConvert.DeserializeObject<UnityMultiRoom>(serverMessage.Content);
                isInRoom = true;
                //JoinRoomEvent?.Invoke(roomData.Settings.RoomName);
            }
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
    /// <summary>
    /// Coroutine that send ping to server every second to check response time
    /// </summary>
    /// <returns></returns>
    public IEnumerator SendPing()
    {
        while (IsConnected)
        {
            Message pingMessage = new Message(MessageType.PING, GetTimeNow().ToString(), GetTimeNow(), clientData.UserID);

            SendMessage(pingMessage);

            setTimeStamp(GetTimeNow());
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    /// <summary>
    /// Method used to get current time
    /// </summary>
    /// <returns>returns time now as long</returns>
    private long GetTimeNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private void IsApplicationPlaying()
    {
        isAppPlaying = Application.isPlaying;
    }


}

