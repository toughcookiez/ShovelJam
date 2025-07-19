using UnityEngine;
using UnityEngine.UI;

public class LevelStateController : MonoBehaviour
{

    [SerializeField] private Level _level;

    private Button _levelButton;

    [SerializeField] private GameObject Lock;

    private void Start()
    {
        _levelButton = GetComponent<Button>();

        if (_level !=  null && _levelButton != null)
        {
            if (_level.isOpen)
            {
                Lock.SetActive(false);
                _levelButton.interactable = true;
            }
            else
            {
                Lock.SetActive(true);
                _levelButton.interactable = false;
            }
        }
        else
        {
            Debug.LogWarning("_level Scriptable object is missing");
        }
    }
}
