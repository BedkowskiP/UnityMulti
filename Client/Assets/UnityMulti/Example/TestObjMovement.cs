using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjMovement : MonoBehaviour
{
    UnityMultiObject obj;

    void Start()
    {
        obj = this.gameObject.GetComponent<UnityMultiObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (obj.IsMine())
        {
            Movement();
        }
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        this.transform.position = new Vector3(horizontalInput, 0, verticalInput) * Time.deltaTime;
    }
}
