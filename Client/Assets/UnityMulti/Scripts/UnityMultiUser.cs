using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnityMultiUser : UnityMultiSerializer<UnityMultiUserHelper>
{
    public UnityMultiUser() { }

    [SerializeField]
    public string Username;
    [SerializeField]
    public string UserID;

    [JsonIgnore]
    public List<GameObject> UserObjectList = new List<GameObject>();

    public void SetParams(UnityMultiUser newPar)
    {
        Username = newPar.Username;
        UserID = newPar.UserID;
    }
    public void SetUserId(string userId)
    {
        UserID = userId;
    }
    public void SetUsername(string username)
    {
        Username = username;
    }
    public string GetID()
    {
        return UserID;
    }

    public override string Serialize(UnityMultiUserHelper obj)
    {
        return base.Serialize(obj);
    }

    public override void Deserialize(string obj)
    {
        base.Deserialize(obj);
        this.Username = temp.Username;
        this.UserID = temp.UserID;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        UnityMultiUser other = (UnityMultiUser)obj;
        return UserID == other.UserID && Username == other.Username;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserID, Username);
    }
    public override string ToString()
    {
        return $"UnityMultiUser - Username: {Username}, UserID: {UserID}";
    }

    public UnityMultiUserHelper ToHelper()
    {
        return new UnityMultiUserHelper(Username, UserID);
    }

}

public class UnityMultiUserHelper
{
    public string Username { get; set; }
    public string UserID { get; set; }

    public UnityMultiUserHelper(UnityMultiUser user)
    {
        this.Username = user.Username;
    }

    [JsonConstructor]
    public UnityMultiUserHelper(string username, string userID)
    {
        this.Username = username;
        this.UserID = userID;
    }
}