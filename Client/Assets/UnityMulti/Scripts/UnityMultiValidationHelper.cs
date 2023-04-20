using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityMultiValidationHelper
{
    public enum ErrorCode
    {
        None = 0,
        InvalidCredientials = 1
    }

    public class ValidationResult
    {
        public bool Validated { get; private set; }
        public ErrorCode ErrorCode { get; private set; }
        public string Username { get; private set; }
        public string UserID { get; private set; }

        public ValidationResult(string userID, string username, bool validated, ErrorCode errorCode)
        {
            Validated = validated;
            ErrorCode = errorCode;
            Username = username;
            UserID = userID;
        }

    }

    public static string ValidationError(ValidationResult validationResult)
    {
        switch (validationResult.ErrorCode)
        {
            case ErrorCode.None:
                return null;
            case ErrorCode.InvalidCredientials:
                return "Invalid Credientials";
        }
        return null;
    }
}