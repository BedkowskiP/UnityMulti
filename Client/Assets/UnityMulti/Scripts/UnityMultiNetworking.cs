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

    public delegate void ServerMessageE(Message serverMessage);
    public event ServerMessageE CustomMessage;

    public delegate void ErrorE(ErrorEventArgs error);
    public event ErrorE ClientError;

    public delegate void ConnectedE();
    public event ConnectedE ClientConnectedAndReady;

    public delegate void DisconnectedE(CloseEventArgs close);
    public event DisconnectedE ClientDisconnected;

    public delegate void ConnectionStateE();
    public event ConnectionStateE ConnectionStateChange;

    public delegate void InitialConnectionE();
    public event InitialConnectionE InitialConnection;

    public delegate void ValidationErrorE(ErrorCode errorCode, string ErrorMessage);
    public event ValidationErrorE ValidationError;

    private delegate void ReconnectE(CloseEventArgs close);
    private event ReconnectE ReconnectEvent;

    public delegate void RoomE();
    public event RoomE JoinRoomEvent;

    public delegate void RoomFailedE(string error);
    public event RoomFailedE CreateRoomFailed;

    public delegate void RoomClientChangeE(UnityMultiUser user);
    public event RoomClientChangeE ClientJoin;
    public event RoomClientChangeE ClientLeave;

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
                InitialConnection?.Invoke();
                ConnectionStateChange?.Invoke();
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
                ClientError?.Invoke(error);
                ConnectionStateChange?.Invoke();
            });
        };

        ws.OnClose += (sender, close) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                ClientDisconnected?.Invoke(close);
                ConnectionStateChange?.Invoke();
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
        Message validationRequest = new Message();
        validationRequest.Type = MessageType.VALIDATION_REQUEST;
        validationRequest.Content = JsonConvert.SerializeObject(clientData);
        validationRequest.Timestamp = GetTimeNow();

        SendMessage(validationRequest);
    }

    public void CreateRoom(UnityMultiRoomSettings settings)
    {
        Debug.Log("IsVakudated: " + isValidated);
        if (isValidated)
        {
            Message roomSettings = new Message(MessageType.CREATE_ROOM, JsonConvert.SerializeObject(settings), GetTimeNow());
            Debug.Log("Settings: " + JsonConvert.SerializeObject(settings));
            SendMessage(roomSettings);
        }
        else
        {
            CreateRoomFailed?.Invoke("Client isn't ready to join the room yet.");
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
                    HandlePong(serverMessage.Content);
                    break;
                case MessageType.VALIDATION_RESPONSE:
                    HandleValidation(serverMessage.Content);
                    break;
                case MessageType.CREATE_ROOM_RESPONSE:
                    HandleCreateRoom(serverMessage.Content);
                    break;
                case MessageType.GAME_STATE:
                    // handle game state message
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
                        CustomMessage?.Invoke(serverMessage);
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
    private void HandlePong(string serverMessage)
    {
        latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - pingTimestamp;
    }

    /// <summary>
    /// validation handler
    /// </summary>
    /// <param name="serverMessage"></param>
    private void HandleValidation(string serverMessage)
    {
        UnityMultiValidationHelper.ValidationResult validationMessage = JsonConvert.DeserializeObject<UnityMultiValidationHelper.ValidationResult>(serverMessage);
        clientData.SetUserId(validationMessage.UserID);

        if (validationMessage.Validated)
        {
            isValidated = true;
            ClientConnectedAndReady?.Invoke();
            StartCoroutine(SendPing());
        }
        else
        {
            ValidationError?.Invoke(validationMessage.ErrorCode, UnityMultiValidationHelper.ValidationError(validationMessage));
            Disconnect();
        }
    }
    private void HandleCreateRoom(string serverMessage)
    {

    }

    public void ClientJoinM(UnityMultiUser user)
    {
        ClientJoin?.Invoke(user);
    }
    public void ClientLeaveM(UnityMultiUser user)
    {
        ClientLeave?.Invoke(user);
    }
    /// <summary>
    /// Coroutine that send ping to server every second to check response time
    /// </summary>
    /// <returns></returns>
    public IEnumerator SendPing()
    {
        while (IsConnected)
        {
            Message pingMessage = new Message(MessageType.PING, GetTimeNow().ToString(), GetTimeNow());

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

