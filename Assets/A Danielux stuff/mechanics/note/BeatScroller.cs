using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        beatTempo = beatTempo / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * Time.deltaTime * beatTempo;
        
    }
}
