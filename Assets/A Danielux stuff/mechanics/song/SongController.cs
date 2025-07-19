using UnityEngine;
using UnityEngine.UI;

public class SongController : MonoBehaviour
{
    [Header("MUST HAVE")]
    public float delay;
    private AudioSource AS;
    public AudioClip Song;
    public Slider SongTimer;


    [Header("Debug")]
    public float SongLength;
    public float SongTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SongTimer.minValue = 0f;
        SongTimer.maxValue = Song.length;
        AS = GetComponent<AudioSource>();
        SongLength = Song.length;

        Invoke("StartSong", delay);
    }

    // Update is called once per frame
    void Update()
    {
        SongTimer.value += Time.timeScale * Time.deltaTime;

        if(SongTimer.value >= Song.length)
        {
            Debug.Log("LevelEnded");
        }
    }

    void StartSong()
    {
        AS.PlayOneShot(Song);

    }



   
}
