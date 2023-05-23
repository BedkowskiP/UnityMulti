using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageType
{
    public const string PING = "ping";
    public const string PONG = "pong";
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
    public const string VALIDATION_REQUEST = "validationRequest";
    public const string VALIDATION_RESPONSE = "validationResponse";
    public const string ADD_UNITY_OBJECT = "addUnityObject";
    public const string ADD_UNITY_OBJECT_RESPONSE = "responseAddUnityObject";
    public const string TRANSFORM_UPDATE = "transformUpdate";

    public const string CONNECT = "connect";
    public const string DISCONNECT = "disconnect";
    public const string GAME_STATE = "gameState";
    public const string PLAYER_POSITION = "playerPosition";
    public const string PLAYER_ROTATION = "playerRotation";
    public const string PLAYER_SCALE = "playerScale";
    public const string SERVER_STATUS = "serverStatus";
    public const string CHAT_MESSAGE = "chatMessage";
    public const string SERVER_MESSAGE = "serverMessage";
    public static List<string> CUSTOM { get; set; }
}

public class Message
{

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
