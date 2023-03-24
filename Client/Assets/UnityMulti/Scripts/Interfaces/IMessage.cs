using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageType
{
    public const string GET_USER_DATA = "getUserData";
    public const string USER_DATA = "userData";
    public const string REQUEST_USER_DATA = "requestUserData";
    public string CUSTOM { get; set; }
}

public interface IMessage
{
    public string Type { get; set; }
    public string Content { get; set; }
}

public class Message : IMessage
{
    public string Type { get; set; }
    public string Content { get; set; }

    public Message(string type, string content)
    {
        this.Type = type;
        this.Content = content;
    }
    public Message(string type)
    {
        this.Type = type;
    }

    public Message()
    {

    }

    
}
