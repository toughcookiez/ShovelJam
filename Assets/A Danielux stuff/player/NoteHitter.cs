using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteHitter : MonoBehaviour
{

    public D11Controls controls;

    [Header("Buttons")]
    public GameObject Left; 
    public GameObject Up;
    public GameObject Down;
    public GameObject Right;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new D11Controls();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Player.left.performed += ctx => LeftPressed();
        controls.Player.up.performed += ctx => UpPressed();
        controls.Player.down.performed += ctx => DownPressed();
        controls.Player.right.performed += ctx => RightPressed();
    }
    
    void Update()
    {
        
    }




    void LeftPressed()
    {
        Debug.Log("L");
        Left.GetComponent<ButtonControl>().Hit();
    }
    void UpPressed()
    {
        Debug.Log("U");
        Up.GetComponent<ButtonControl>().Hit();
    }
    void DownPressed()
    {
        Debug.Log("D");
        Down.GetComponent<ButtonControl>().Hit();
    }
    void RightPressed()
    {
        Debug.Log("P");
        Right.GetComponent<ButtonControl>().Hit();
    }
}
