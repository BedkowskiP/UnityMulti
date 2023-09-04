using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleObject : MonoBehaviour
{
    public UnityMultiObject obj;
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        obj = GetComponent<UnityMultiObject>();
    }

    void Update()
    {
        if(obj != null)
            if (obj.IsMine())
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    RollRandomColorBuff();
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    RollRandomColorAll();
                }
                Movement();
            }
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * 3f * Time.deltaTime);
    }

    private void RollRandomColorBuff()
    {
        float r = Random.value;
        float g = Random.value;
        float b = Random.value;

        Color randomColor = new Color(r, g, b);

        obj.RunRPC("ChangeColor", RPCTarget.Buffered, randomColor);
    }

    private void RollRandomColorAll()
    {
        float r = Random.value;
        float g = Random.value;
        float b = Random.value;

        Color randomColor = new Color(r, g, b);

        obj.RunRPC("ChangeColor", RPCTarget.All, randomColor);
    }

    [UnityMultiRPC]
    public void ChangeColor(Color color)
    {
        objectRenderer.material.color = color;
    }
}
