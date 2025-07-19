using UnityEngine;

public class BandController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer Piano, Guitar, Drums;

    [SerializeField] private Sprite _pianoIdle, _piano1, _piano2;

    [SerializeField] private Sprite _guitarIdle, _guitar1, _guitar2;

    [SerializeField] private Sprite _drumsIdle, _drums1, _drums2;

    private int _frameCount = 0;

    private void Start()
    {
        Piano.sprite = _pianoIdle;
        Guitar.sprite = _guitarIdle;
        Drums.sprite = _drumsIdle;
    }

    public void UpdateSprites()
    {
        _frameCount++;
        if (_frameCount > 2)
        {
            _frameCount = 1;
        }
        if (_frameCount == 1)
        {
            Piano.sprite = _piano1;
            Guitar.sprite = _guitar1;
            Drums.sprite = _drums1;
        }
        else if (_frameCount == 2)
        {
            Piano.sprite = _piano2;
            Guitar.sprite = _guitar2;
            Drums.sprite = _drums2;
        }
        
    }




}
