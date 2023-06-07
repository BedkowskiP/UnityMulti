using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjMovement : MonoBehaviour
{
    UnityMultiObject obj;
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        obj = GetComponent<UnityMultiObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (obj.IsMine())
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                RollRandomColor();
            }
            Movement();
        }
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * 3f * Time.deltaTime);
    }

    private void RollRandomColor()
    {
        float r = Random.value;
        float g = Random.value;
        float b = Random.value;

        Color randomColor = new Color(r, g, b);

        obj.RunRPC("ChangeColor", RPCTarget.Buffered, randomColor);
    }

    [UnityMultiRPC]
    public void ChangeColor(Color color)
    {
        objectRenderer.material.color = color;
    }
}
