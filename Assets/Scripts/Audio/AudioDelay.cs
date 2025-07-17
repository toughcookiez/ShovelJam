using System.Threading;
using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    public float _songDelay;

    private bool _isPlaying;

    private float _timer;

    private AudioSource _source;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_isPlaying)
        {
            return;
        }

        if (_timer < _songDelay && _songDelay != 0)
        {
            _timer += Time.deltaTime;
            return;
        }
        else
        {
            _isPlaying = true;
            _source.Play();
        }
    }
}
