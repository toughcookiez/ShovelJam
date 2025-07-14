using System.Collections;
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
    public GameObject note;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }


















    void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.transform.CompareTag("note"))
        {
            note = other.gameObject;
            isHitting = true;

        }
    }
     void OnTriggerExit2D(Collider2D collision)
    {
        isHitting = false;
        note = null;
    }
















    public void Hit()
    {
        StartCoroutine(HitNote());
    }
    public IEnumerator HitNote()
    {

       
        if(isHitting)
        {
            Destroy(note.gameObject);
           
            sr.sprite = NoteHit;
            


        }else if(!isHitting)
        {
            sr.sprite = NoteMiss;
        }

        yield return new WaitForSeconds(0.1f);
        sr.sprite = Default;
    }

   
}
