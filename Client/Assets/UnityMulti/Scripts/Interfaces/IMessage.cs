using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageType
{
    /// <summary>
    /// these messages are used check latency.
    /// </summary>
    public const string PING = "ping";
    public const string PONG = "pong";
    /// <summary>
    /// 
    /// </summary>
    public const string CREATE_ROOM = "createRoom";
    public const string JOIN_ROOM = "joinRoom";
    public const string CREATE_ROOM_RESPONSE = "responseCreateRoom";
    public const string JOIN_ROOM_RESPONSE = "responseJoinRoom";
    public const string USER_JOIN = "userJoin";
    public const string USER_LEAVE = "userLeave";
    public const string LEAVE_ROOM = "leaveRoom";
    public const string LEAVE_ROOM_RESPONSE = "responseLeaveRoom";
    public const string HOST_CHANGE = "hostChange";
    public const string HOST_CHANGE_RESPONSE = "responseHostChange";
    /// <summary>
    /// these message types could be used to establisz connection between client and server.
    /// </summary>
    public const string CONNECT = "connect";
    public const string DISCONNECT = "disconnect";
    /// <summary>
    /// these message types could be used to request/send user data from/to server.
    /// </summary>
    public const string VALIDATION_REQUEST = "validationRequest";
    public const string VALIDATION_RESPONSE = "validationResponse";
    /// <summary>
    /// something like closed instantions of the game (e.g. dungeons, missions)?
    /// </summary>
    public const string GAME_STATE = "gameState";
    /// <summary>
    /// these message types could be used to send player position, rotation and scale.
    /// </summary>
    public const string PLAYER_POSITION = "playerPosition";
    public const string PLAYER_ROTATION = "playerRotation";
    public const string PLAYER_SCALE = "playerScale";
    /// <summary>
    /// this message type could be used to send information about the server's status (e.g. number of clients connected, server load) to the clients.
    /// </summary>
    public const string SERVER_STATUS = "serverStatus";
    /// <summary>
    /// this message type could be used to implement chat.
    /// </summary>
    public const string CHAT_MESSAGE = "chatMessage";
    /// <summary>
    /// this message type could be used to send server related information to clients (e.g. server maintenance, scheduled downtime, etc.).
    /// </summary>
    public const string SERVER_MESSAGE = "serverMessage";
    /// <summary>
    /// this message type could be used to make custom message types that aren't included as base message types.
    /// </summary> 
    public static List<string> CUSTOM { get; set; }
}

public interface IMessage
{
    public string Type { get; set; }
    public string Content { get; set; }
    public long? Timestamp { get; set; }
    public ErrorCode ErrorCode { get; set; }
    public string UserID { get; set; }
}

public class Message : IMessage
{
    private string vALIDATION_REQUEST;
    private string v1;
    private long v2;

    public string Type { get; set; }
    public string Content { get; set; }
    public long? Timestamp { get; set; } = null;
    public ErrorCode ErrorCode { get; set; } = 0;
    public string UserID { get; set; }

    [JsonConstructor]
    public Message(string type, string content, long timestamp, ErrorCode errorCode)
    {
        this.Type = type;
        this.Content = content;
        this.Timestamp = timestamp;
        this.ErrorCode = errorCode;
    }


    public Message(string type, string content, long timestamp, string UserID)
    {
        this.Type = type;
        this.Content = content;
        this.Timestamp = timestamp;
        this.UserID = UserID;
    }
    public Message(string type, string content)
    {
        this.Type = type;
        this.Content = content;
    }
}
