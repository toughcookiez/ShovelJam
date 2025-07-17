// TimedTextEntry.cs
// This script defines a simple data structure to hold a timestamp and its associated text.
// It needs to be marked as [System.Serializable] so Unity can serialize it and display it
// in the Inspector, especially when it's part of a List or array.

[System.Serializable]
public class AudioTranscriptionEvent
{
    // The timestamp in seconds for this entry.
    public float timestamp;

    // The text associated with this timestamp.
    public int lane;

    // Constructor to easily create new entries.
    public AudioTranscriptionEvent(float time, int lane)
    {
        this.timestamp = time;
        this.lane = lane;
    }
}