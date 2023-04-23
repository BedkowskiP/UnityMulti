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
            default:
                return "Invalid error code";
        }

        return "";
    }
}
