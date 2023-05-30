using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class CreateObjectOnNewScene : UnityMultiNetworkingCallbacks
{
    public Text ping;

    public override void OnClientJoin(UnityMultiUser user)
    {
        base.OnClientJoin(user);
        Debug.Log("User " + user.Username + " joined the room.");
        multiNetworking.InstantiatePlayerObject("TutorialBall", new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1, 1, 1), user);
    }

    public override void OnClientLeave(UnityMultiUser user)
    {
        base.OnClientLeave(user);
        Debug.Log("User: " + user.Username + " left the room.");
    }

    private void Update()
    {
        ping.text = "Ping: " + multiNetworking.GetLatency().ToString();
    }
}
