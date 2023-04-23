using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidationResult
{
    public bool Validated { get; private set; }
    public string Username { get; private set; }
    public string UserID { get; private set; }

    public ValidationResult(string userID, string username, bool validated)
    {
        Validated = validated;
        Username = username;
        UserID = userID;
    }

}
