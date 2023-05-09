using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMultiUser
{
    public string Username { get; private set; } = "";
    public string UserID { get; private set; } = "";

    public void SetUserId(string userId)
    {
        UserID = userId;
    }
    public void SetUserName(string username)
    {
        Username = username;
    }

    public UnityMultiUser() { }
    public UnityMultiUser(string username)
    {
        this.Username = username;
    }
    [JsonConstructor]
    public UnityMultiUser(string username, string userID)
    {
        this.Username = username;
        this.UserID = userID;
    }
}
