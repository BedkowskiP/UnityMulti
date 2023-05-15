using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class CreateObjectOnNewScene : UnityMultiNetworkingCallbacks
{
    public override void OnClientJoin(UnityMultiUser user)
    {
        base.OnClientJoin(user);
        Debug.Log("User " + user.Username + " joined the room.");
        multiNetworking.InstantiatePlayerObject("TutorialBall", new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1,1,1), null);
    }

}
