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
        Left.GetComponent<ButtonControl>().Hit();
    }
    void UpPressed()
    {
        Up.GetComponent<ButtonControl>().Hit();
    }
    void DownPressed()
    {
        Down.GetComponent<ButtonControl>().Hit();
    }
    void RightPressed()
    {
        Right.GetComponent<ButtonControl>().Hit();
    }
}
