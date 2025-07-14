using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("LEROOOOOOOOOOOOOYYYYYYY JJJJJJJEEIIIIIIIIKKKKKKKKKIIIIIIIIIIIIIIIIIIIIIINNNNNNNNNNSSSSSSSS");
        if (other.tag == ("Player"))
        {

            Debug.Log("YYYYYYYYYYYYYYYYYYYYYEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEHAW!!!!!!!!!!!!!!!");

        }
    }















   
}
