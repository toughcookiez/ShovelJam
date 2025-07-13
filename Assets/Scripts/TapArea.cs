using System;
using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class TapArea : MonoBehaviour
{
    public string _buttonLetter = "w";

    private Collider2D _collider;

    private ControlsScript _controls;

    private void Awake()
    {

        Controls.Enable();

        if (string.Equals(_buttonLetter, "w", StringComparison.InvariantCultureIgnoreCase))
        {
            Debug.Log("w");
            Controls.Controls.W.started += OnTap;
        }
        if (string.Equals(_buttonLetter, "a", StringComparison.InvariantCultureIgnoreCase))
        {
            Debug.Log("a");
            Controls.Controls.A.started += OnTap;
        }
        if (string.Equals(_buttonLetter, "s", StringComparison.InvariantCultureIgnoreCase))
        {
            Debug.Log("s");
            Controls.Controls.S.started += OnTap;
        }
        if (string.Equals(_buttonLetter, "d", StringComparison.InvariantCultureIgnoreCase))
        {
            Debug.Log("d");
            Controls.Controls.D.started += OnTap;
        }
    }

    private ControlsScript Controls
    {
        get
        {
            if (_controls == null)
            {
                _controls = new ControlsScript();
            }
            return _controls;
        }
    }

    void OnEnable()
    {
        Controls.Enable();
    }

    void OnDisable()
    {
        Controls.Disable();
    }

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTap(InputAction.CallbackContext ob)
    {
        Debug.Log("f");
        StartCoroutine(TapCollider());
    }

  
    private IEnumerator TapCollider()
    {
        _collider.enabled = true;
        yield return new WaitForSeconds(.01f);
        _collider.enabled = false;
        
    }
}
