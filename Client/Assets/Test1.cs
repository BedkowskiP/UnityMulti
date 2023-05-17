using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class UCallbacks : MonoBehaviour
{
    protected void Awake()
    {
        Debug.Log("Aw1");
    }
}

public class Test1 : UCallbacks
{
    private void Awake()
    {
        Debug.Log("Aw2");
    }
}