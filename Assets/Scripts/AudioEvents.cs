using UnityEngine;

public class AudioEvents : MonoBehaviour
{
    [Tooltip("The AudioSource component to analyze.")]
    public AudioSource _audioSource;

    [Tooltip("Multiplier for detecting a beat. A higher value means the current amplitude must be significantly louder than the average to be considered a beat.")]
    [Range(1.0f, 3.0f)] // Recommended range for tuning
    public float _beatThreshold = 1.2f;

    [Tooltip("Minimum time in seconds between detected beats. Prevents multiple beat detections from a single transient.")]
    [Range(0.05f, 1.0f)] // Recommended range for tuning
    public float _beatCooldownTime = 0.2f;

    // Internal array to hold the raw spectrum data from AudioSource.GetSpectrumData
    private float[] _spectrum;

    // History buffer to store recent amplitude values for calculating the average
    private float[] _historyBuffer;
    [Tooltip("Number of frames to keep in the history buffer for calculating the average amplitude. Influences the 'reactiveness' of the beat detection.")]
    [Range(30, 300)] // For 60 FPS, 60 frames = 1 second of history
    public int _historyBufferSize = 60; // Default to 1 second of history at 60 FPS

    private int _historyBufferIndex = 0; // Current index in the history buffer

    private float _currentAmplitude;  // The calculated amplitude of the current frame
    private float _averageAmplitude;  // The calculated average amplitude from the history buffer
    private float _lastBeatTime;      // The Time.time when the last beat was detected

    private bool _isBeatThisFrame = false; // Internal flag indicating if a beat occurred in the current frame

    // Constants for musical note calculation
    private const float A4_FREQUENCY = 440.0f; // Frequency of A4 note
    private const int A4_MIDI_NOTE = 69;       // MIDI note number for A4
    private static readonly string[] NOTE_NAMES = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    /// <summary>
    /// Public property to check if a beat was detected in the current frame.
    /// This will be true for one frame when a beat occurs.
    /// </summary>
    public bool IsBeat
    {
        get { return _isBeatThisFrame; }
    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the AudioSource and data arrays.
    /// </summary>
    void Start()
    {
        // If AudioSource is not assigned in the Inspector, try to get it from this GameObject
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                Debug.LogError("BeatDetector: No AudioSource found on this GameObject. Please assign one or add an AudioSource component.");
                enabled = false; // Disable the script if no AudioSource is found
                return;
            }
        }

        // Initialize the spectrum array. Its size must be a power of 2.
        // 1024 is a good balance between performance and frequency resolution.
        _spectrum = new float[1024];

        // Initialize the history buffer for amplitude averaging
        _historyBuffer = new float[_historyBufferSize];
        // Initialize history buffer with zeros
        for (int i = 0; i < _historyBufferSize; i++)
        {
            _historyBuffer[i] = 0f;
        }

        // Initialize _lastBeatTime to allow a beat to be detected immediately at start
        _lastBeatTime = -_beatCooldownTime;
    }

    /// <summary>
    /// Called once per frame.
    /// Performs spectrum analysis and beat detection.
    /// </summary>
    void Update()
    {
       if (IsBeatFrame)
        {
            int lane = GetDominantMusicalNoteLane(4);
            if (lane > 0)
            {
                Debug.Log("Beat:" + _lastBeatTime + " " + lane + " " + GetDominantMusicalNote());
            }
        }
    }

    private bool IsBeatFrame
    {
        get
        {
            // Reset the beat flag for the current frame
            _isBeatThisFrame = false;

            // 1. Get the audio spectrum data
            // _spectrum: The array to populate with frequency domain data.
            // 0: The channel to sample from (0 for master/mixed mono).
            // FFTWindow.BlackmanHarris: A windowing function to reduce spectral leakage.
            _audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

            // 2. Calculate the current overall amplitude (energy) from the spectrum data
            _currentAmplitude = 0f;
            for (int i = 0; i < _spectrum.Length; i++)
            {
                // Summing the magnitude of each frequency bin
                _currentAmplitude += _spectrum[i];
            }
            // Average the sum to get a representative amplitude value
            _currentAmplitude /= _spectrum.Length;

            // 3. Update the history buffer with the current amplitude
            _historyBuffer[_historyBufferIndex] = _currentAmplitude;
            // Move to the next index, wrapping around if at the end of the buffer
            _historyBufferIndex = (_historyBufferIndex + 1) % _historyBufferSize;

            // 4. Calculate the average amplitude from the history buffer
            _averageAmplitude = 0f;
            for (int i = 0; i < _historyBufferSize; i++)
            {
                _averageAmplitude += _historyBuffer[i];
            }
            _averageAmplitude /= _historyBufferSize;

            // 5. Beat detection logic
            // Check if enough time has passed since the last detected beat
            if (Time.time - _lastBeatTime >= _beatCooldownTime)
            {
                // If the current amplitude is significantly higher than the average amplitude (by _beatThreshold)
                if (_currentAmplitude > _averageAmplitude * _beatThreshold)
                {
                    _isBeatThisFrame = true; // Set the beat flag to true for this frame
                    _lastBeatTime = Time.time; // Record the time of this beat
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Optional: Visualizes the spectrum data in the Scene view for debugging.
    /// </summary>
    void OnDrawGizmos()
    {
        if (_spectrum == null || _spectrum.Length == 0) return;

        Gizmos.color = Color.red;
        float scaleFactor = 100f; // Adjust this to change the height of the lines
        float spacing = 0.01f;   // Adjust this to change the spacing between lines

        // Draw lines representing the spectrum data
        for (int i = 0; i < _spectrum.Length; i++)
        {
            Vector3 start = transform.position + new Vector3(i * spacing - (_spectrum.Length * spacing / 2f), 0, 0);
            Vector3 end = start + new Vector3(0, _spectrum[i] * scaleFactor, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    /// <summary>
    /// Finds the dominant frequency in the spectrum and converts it to a musical note name.
    /// </summary>
    /// <returns>A string representing the dominant musical note (e.g., "C4", "A#5").</returns>
    private string GetDominantMusicalNote()
    {
        if (_spectrum == null || _spectrum.Length == 0)
        {
            return "N/A";
        }

        float maxAmplitude = 0f;
        int maxIndex = 0;

        // Find the frequency bin with the highest amplitude
        for (int i = 0; i < _spectrum.Length; i++)
        {
            if (_spectrum[i] > maxAmplitude)
            {
                maxAmplitude = _spectrum[i];
                maxIndex = i;
            }
        }

        // If no significant amplitude is found, return N/A
        if (maxAmplitude < 0.001f) // A small threshold to avoid noise
        {
            return "N/A";
        }

        // Calculate the frequency corresponding to the maxIndex
        // Frequency per bin = (AudioSettings.outputSampleRate / 2) / _spectrum.Length
        // The Nyquist frequency (half the sample rate) is the max frequency represented.
        float frequency = maxIndex * (AudioSettings.outputSampleRate / 2.0f) / _spectrum.Length;

        // Convert frequency to MIDI note number
        // MIDI note number = 69 + 12 * log2(frequency / 440 Hz)
        float midiNoteFloat = A4_MIDI_NOTE + 12 * Mathf.Log(frequency / A4_FREQUENCY, 2);

        // Round to the nearest integer to get the MIDI note number
        int midiNote = Mathf.RoundToInt(midiNoteFloat);

        // Ensure MIDI note is within a reasonable range (0-127)
        midiNote = Mathf.Clamp(midiNote, 0, 127);

        // Calculate octave and note name
        int noteIndex = midiNote % 12; // 0=C, 1=C#, ..., 11=B
        int octave = (midiNote / 12) - 1; // MIDI note 0 is C-1, so C4 is MIDI 60

        return NOTE_NAMES[noteIndex] + octave.ToString();
    }



    private int GetDominantMusicalNoteLane(int numberOfLanes)
    {
        if (_spectrum == null || _spectrum.Length == 0)
        {
            return -1;
        }

        float maxAmplitude = 0f;
        int maxIndex = 0;

        // Find the frequency bin with the highest amplitude
        for (int i = 0; i < _spectrum.Length; i++)
        {
            if (_spectrum[i] > maxAmplitude)
            {
                maxAmplitude = _spectrum[i];
                maxIndex = i;
            }
        }

        // If no significant amplitude is found, return N/A
        if (maxAmplitude < 0.001f) // A small threshold to avoid noise
        {
            return -1;
        }

        // Calculate the frequency corresponding to the maxIndex
        // Frequency per bin = (AudioSettings.outputSampleRate / 2) / _spectrum.Length
        // The Nyquist frequency (half the sample rate) is the max frequency represented.
        float frequency = maxIndex * (AudioSettings.outputSampleRate / 2.0f) / _spectrum.Length;

        // Convert frequency to MIDI note number
        // MIDI note number = 69 + 12 * log2(frequency / 440 Hz)
        float midiNoteFloat = A4_MIDI_NOTE + 12 * Mathf.Log(frequency / A4_FREQUENCY, 2);

        // Round to the nearest integer to get the MIDI note number
        int midiNote = Mathf.RoundToInt(midiNoteFloat);

        // Ensure MIDI note is within a reasonable range (0-127)
        midiNote = Mathf.Clamp(midiNote, 0, 127);

        return midiNote % numberOfLanes;
    }
}
