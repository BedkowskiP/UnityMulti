using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ErrorCode
{
    None = 0,
    InvalidCredientials = 1,
    NotValidated = 2,
    //100=199 validation
    ValidatedCorrectly = 100,
    WrongUsername = 101,
    //200-299 rooms
    AlreadyInRoom = 202,

}

public static class UnityMultiErrorHandler
{
    public static string ErrorMessage(ErrorCode code)
    {
        switch (code)
        {
            case ErrorCode.None:
                break;
            case ErrorCode.NotValidated:
                return "Client isn't ready to join room yet";
            case ErrorCode.WrongUsername:
                return "Wrong username";
            case ErrorCode.AlreadyInRoom:
                return "You are already in room. Try leaving one before joining another";
            default:
                return "Invalid error code";
        }

        return "";
    }
}
