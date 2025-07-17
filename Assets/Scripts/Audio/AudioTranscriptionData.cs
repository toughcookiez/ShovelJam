// AudioTranscriptionData.cs
// This ScriptableObject acts as a data container for an AudioClip and a list of
// TimedTextEntry objects. By making it a ScriptableObject, you can create instances
// of this data directly in your Unity project as assets, which can then be
// selected and edited in the custom editor window.

using UnityEngine;
using System.Collections.Generic;

// The CreateAssetMenu attribute allows you to create new instances of this
// ScriptableObject directly from the Unity Editor's Assets/Create menu.
[CreateAssetMenu(fileName = "NewAudioTranscription", menuName = "Audio/Audio Transcription Data")]
public class AudioTranscriptionData : ScriptableObject
{
    // The AudioClip that this transcription data is associated with.
    public AudioClip audioClip;

    // A list of TimedTextEntry objects, storing all the timestamps and their texts.
    public List<AudioTranscriptionEvent> transcriptionEntries = new List<AudioTranscriptionEvent>();
}