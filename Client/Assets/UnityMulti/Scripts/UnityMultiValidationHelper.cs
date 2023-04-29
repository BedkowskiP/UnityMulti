using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ValidationResult
{
    public bool Validated { get; private set; }
    public string Username { get; private set; }
    public string UserID { get; private set; }
    [JsonConstructor]
    public ValidationResult(string userID, string username, bool validated)
    {
        this.Validated = validated;
        this.Username = username;
        this.UserID = userID;
    }

}
