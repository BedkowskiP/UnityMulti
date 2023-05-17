using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleter : MonoBehaviour
{
    public string nameToDelete;
    public GameObject Prefa;
    private void Update()
    {
        
        
        
      
    }
    void Start()
    {
        GameObject Gamo=Instantiate(Prefa,new Vector3(1,1,1),new Quaternion(1,1,1,1));
        Gamo.name="1";
    }
    private void OnGUI()
     {
         if (GUILayout.Button("Generate Nodes"))
         {
            GameObject objectToDelete = GameObject.Find(nameToDelete);
            DestroyImmediate (objectToDelete, true);
         }
     }
}
