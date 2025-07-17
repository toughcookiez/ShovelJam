using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonControl : MonoBehaviour
{
    SpriteRenderer sr;
    [Header("Sprites")]
    public Sprite NoteHit;
    public Sprite NoteMiss;
    public Sprite Default;

    [Header ("hit detection")]
    public bool isHitting;
    public GameObject _note { get; set; }

    public GameObject _noteExplosion;

    [SerializeField] private Transform VFXHolder;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.transform.CompareTag("note"))
        {
            _note = other.gameObject;
            isHitting = true;

        }
    }
     void OnTriggerExit2D(Collider2D collision)
    {
        isHitting = false;
        _note = null;
    }

    public void Hit()
    {
        if (_note == null)
        {
            return;
        }

        StartCoroutine(HitNote(_note));
        
    }
    public IEnumerator HitNote(GameObject note)
    {

        if (isHitting)
        {

            Instantiate(_noteExplosion, note.transform.position, Quaternion.identity, VFXHolder);
            Destroy(note.gameObject);

            sr.sprite = NoteHit;




        }
        else if (!isHitting)
        {
            sr.sprite = NoteMiss;
        }

        yield return new WaitForSeconds(0.1f);
        sr.sprite = Default;
    }

   
}
