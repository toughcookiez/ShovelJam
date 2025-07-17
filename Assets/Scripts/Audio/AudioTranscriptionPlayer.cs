using UnityEngine;
using System.Collections.Generic;

public class AudioTranscriptionPlayer : MonoBehaviour
{
    [SerializeField] private AudioTranscriptionData _audioTranscription;

    private AudioSource _audioSource;

    [SerializeField] private GameObject _notePrefab;

    [SerializeField] private GameObject _noteHolder;

    [SerializeField] private Transform _targetTransform;

    [SerializeField] private float _silenceDuration;

    private Queue<AudioTranscriptionEvent> _noteQueue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if ( _audioTranscription == null )
        {
            Debug.LogError("Audio transcription data should be valid.");
            return;
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio source should be valid.");
            return;
        }

        if (_notePrefab == null)
        {
            Debug.LogError("Note prefab should be valid.");
            return;
        }

        _noteQueue = new Queue<AudioTranscriptionEvent>();

        List<AudioTranscriptionEvent> sortedList = new List<AudioTranscriptionEvent>(_audioTranscription.transcriptionEntries);
        sortedList.Sort((t1, t2) => t1.timestamp > t2.timestamp ? 1 : t1.timestamp < t2.timestamp ? -1 : 0);
        foreach (AudioTranscriptionEvent entry in sortedList)
        {
            _noteQueue.Enqueue(entry);
        }



        _audioSource.clip = _audioTranscription.audioClip;
        _audioSource.clip = CreateClipWithSilenceIntro(_audioSource.clip, _silenceDuration);
        _audioSource.Play();
    }

    AudioClip CreateClipWithSilenceIntro(AudioClip originalClip, float silenceDuration)
    {
        int originalSamples = originalClip.samples;
        int channels = originalClip.channels;
        int frequency = originalClip.frequency;

        // Calculate silence samples
        int silenceSamples = Mathf.CeilToInt(silenceDuration * frequency);

        // Ensure silenceSamples is an even number for stereo clips, or just ensure it's not negative.
        // It's more critical that the total samples align correctly if you're writing to a file,
        // but for in-memory AudioClip manipulation, Unity usually handles this.
        if (channels == 2 && silenceSamples % 2 != 0)
        {
            silenceSamples++;
        }

        int totalSamples = originalSamples + silenceSamples;

        // Create the new AudioClip
        AudioClip newClip = AudioClip.Create(originalClip.name + "_withSilence", totalSamples, channels, frequency, false);

        // Get original audio data
        float[] originalData = new float[originalSamples * channels];
        originalClip.GetData(originalData, 0);

        // Create combined audio data array
        float[] combinedData = new float[totalSamples * channels];

        // Fill the beginning with silence (zeros) - this is implicitly done as float arrays are initialized to 0
        // for (int i = 0; i < silenceSamples * channels; i++)
        // {
        //     combinedData[i] = 0f;
        // }

        // Copy original audio data after the silence
        System.Array.Copy(originalData, 0, combinedData, silenceSamples * channels, originalData.Length);

        // Set the combined data to the new AudioClip
        newClip.SetData(combinedData, 0);

        return newClip;
    }

    // Update is called once per frame
    void Update()
    {
        if (_audioTranscription == null)
        {
            return;
        }

        if (_audioSource == null)
        {
            return;
        }

        if (_notePrefab == null)
        {
            return;
        }

        if (_noteQueue.Count == 0)
        {
            return;
        }


        AudioTranscriptionEvent currentEntry = _noteQueue.Peek();
        if (currentEntry == null )
        {
            return;
        }

        if (_noteHolder == null)
        {
            return;
        }

        if (_audioSource.time >= currentEntry.timestamp)
        {
            _noteQueue.Dequeue();
            //Debug.Log("Entry time: " + currentEntry.timestamp + ", source time=" + _audioSource.time + ", lane=" + currentEntry.lane);
            GameObject note = Instantiate(_notePrefab, transform.position + (Vector3.left * currentEntry.lane * 2), Quaternion.identity, _noteHolder.transform);
            note.GetComponent<note>()._targetTransform = _targetTransform;
        }
        
    }
}
