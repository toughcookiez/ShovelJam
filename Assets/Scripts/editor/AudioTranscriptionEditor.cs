// AudioTranscriptionEditor.cs
// This script creates a custom EditorWindow in Unity that allows you to:
// 1. Select an AudioTranscriptionData ScriptableObject.
// 2. View the waveform of the associated AudioClip.
// 3. Play, pause, and stop the audio.
// 4. Scrub through the audio using a slider.
// 5. Add new timestamped text entries at the current audio playback position.
// 6. Edit existing timestamped text entries.
// 7. Jump to a specific timestamp by clicking a button next to an entry.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // For OrderBy

public class AudioTranscriptionEditor : EditorWindow
{
    // --- Editor Window Setup ---
    // This method adds a menu item to the Unity Editor's "Window" menu to open this editor.
    [MenuItem("Window/Audio Transcription Editor")]
    public static void ShowWindow()
    {
        // Get existing open window or create a new one.
        GetWindow<AudioTranscriptionEditor>("Audio Transcription");
    }

    // --- Data and State Variables ---
    private AudioTranscriptionData _currentTranscriptionData; // The ScriptableObject being edited
    private SerializedObject _serializedObject; // Used for proper Undo/Redo and saving changes
    private SerializedProperty _transcriptionEntriesProperty; // Reference to the list of entries

    private AudioClip _currentAudioClip; // The audio clip from _currentTranscriptionData
    private AudioSource _audioSource; // Used for playing audio in the editor
    private GameObject _audioSourceGO; // Temporary GameObject to hold the AudioSource

    private Texture2D _waveformTexture; // Texture to display the audio waveform
    private float _waveformHeight = 100f; // Height of the waveform display
    private float _playbackTime = 0f; // Current playback time of the audio
    private bool _isPlaying = false; // Is audio currently playing?

    private Vector2 _scrollPosition; // Scroll position for the transcription entries list

    // --- Editor Window Lifecycle Methods ---

    // Called when the editor window is enabled or opened.
    private void OnEnable()
    {
        // Subscribe to EditorApplication.update for continuous updates (e.g., playback time).
        EditorApplication.update += Update;
        // Restore previous selection if any
        if (Selection.activeObject is AudioTranscriptionData)
        {
            SetTranscriptionData(Selection.activeObject as AudioTranscriptionData);
        }
    }

    // Called when the editor window is disabled or closed.
    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks and unnecessary updates.
        EditorApplication.update -= Update;
        StopAudio(); // Ensure audio is stopped when closing the window
        CleanupAudioSource(); // Clean up the temporary GameObject
    }

    // Called frequently to update the editor window's UI.
    private void OnGUI()
    {
        // --- Select Audio Transcription Data Asset ---
        EditorGUI.BeginChangeCheck();
        AudioTranscriptionData newTranscriptionData = (AudioTranscriptionData)EditorGUILayout.ObjectField(
            "Transcription Data", _currentTranscriptionData, typeof(AudioTranscriptionData), false);
        if (EditorGUI.EndChangeCheck())
        {
            SetTranscriptionData(newTranscriptionData);
        }

        if (_currentTranscriptionData == null)
        {
            EditorGUILayout.HelpBox("Please select an 'Audio Transcription Data' asset to begin.", MessageType.Info);
            return; // Nothing to do without data
        }

        // --- Display Audio Clip ---
        EditorGUILayout.Space();
        EditorGUILayout.ObjectField("Audio Clip", _currentAudioClip, typeof(AudioClip), false);

        if (_currentAudioClip == null)
        {
            EditorGUILayout.HelpBox("The selected 'Audio Transcription Data' asset has no AudioClip assigned.", MessageType.Warning);
            return; // Cannot proceed without an audio clip
        }

        // --- Waveform Display ---
        DrawWaveform();

        // --- Playback Controls ---
        DrawPlaybackControls();

        // --- Transcription Entries List ---
        DrawTranscriptionEntries();

        // Apply any modified properties to the SerializedObject for saving and Undo/Redo.
        // This is crucial to ensure changes made via SerializedProperty are written back to the actual object.
        if (_serializedObject != null && _serializedObject.targetObject != null)
        {
            _serializedObject.ApplyModifiedProperties();
        }

      
    }

    // --- Helper Methods ---

    // Sets the current AudioTranscriptionData asset and initializes related variables.
    private void SetTranscriptionData(AudioTranscriptionData data)
    {
        if (_currentTranscriptionData != data)
        {
            StopAudio(); // Stop any currently playing audio
            CleanupAudioSource(); // Clean up previous audio source

            _currentTranscriptionData = data;
            if (_currentTranscriptionData != null)
            {
                _serializedObject = new SerializedObject(_currentTranscriptionData);
                _transcriptionEntriesProperty = _serializedObject.FindProperty("transcriptionEntries");
                _currentAudioClip = _currentTranscriptionData.audioClip;
                GenerateWaveformTexture(); // Regenerate waveform for new clip
                InitializeAudioSource(); // Initialize audio source for new clip
            }
            else
            {
                _serializedObject = null;
                _transcriptionEntriesProperty = null;
                _currentAudioClip = null;
                _waveformTexture = null;
            }
            Repaint(); // Redraw the window
        }
    }

    // Generates a Texture2D representing the audio waveform.
    private void GenerateWaveformTexture()
    {
        if (_currentAudioClip == null)
        {
            _waveformTexture = null;
            return;
        }

        // Ensure the audio clip is loaded and readable.
        // For compressed audio, Load Type must be "Decompress on Load" in the AudioClip importer settings.
        if (!_currentAudioClip.LoadAudioData())
        {
            Debug.LogError("Failed to load audio data for waveform. Check AudioClip import settings (Load Type: Decompress on Load).");
            _waveformTexture = null;
            return;
        }

        int width = (int)position.width; // Use current window width for texture width
        int height = (int)_waveformHeight;
        _waveformTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];

        // Fill background with a light grey
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(0.15f, 0.15f, 0.15f, 1f);
        }

        // Get audio data
        float[] samples = new float[_currentAudioClip.samples * _currentAudioClip.channels];
        _currentAudioClip.GetData(samples, 0);

        // Draw waveform
        float step = (float)samples.Length / width;
        for (int i = 0; i < width; i++)
        {
            float minVal = 1f;
            float maxVal = -1f;
            int startSample = (int)(i * step);
            int endSample = (int)((i + 1) * step);
            if (endSample >= samples.Length) endSample = samples.Length - 1;

            for (int j = startSample; j < endSample; j++)
            {
                minVal = Mathf.Min(minVal, samples[j]);
                maxVal = Mathf.Max(maxVal, samples[j]);
            }

            // Map min/max values to pixel coordinates
            int yMin = Mathf.FloorToInt((minVal * 0.5f + 0.5f) * height);
            int yMax = Mathf.CeilToInt((maxVal * 0.5f + 0.5f) * height);

            // Draw a vertical line for the waveform segment
            for (int y = yMin; y <= yMax; y++)
            {
                if (y >= 0 && y < height)
                {
                    pixels[y * width + i] = Color.cyan; // Waveform color
                }
            }
        }

        _waveformTexture.SetPixels(pixels);
        _waveformTexture.Apply();
    }

    // Draws the waveform texture and the playback indicator.
    private void DrawWaveform()
    {
        if (_waveformTexture == null)
        {
            GenerateWaveformTexture(); // Attempt to generate if null
            if (_waveformTexture == null) return; // Still null, cannot draw
        }

        Rect waveformRect = GUILayoutUtility.GetRect(0, _waveformHeight, GUILayout.ExpandWidth(true));
        GUI.DrawTexture(waveformRect, _waveformTexture);

        // Draw playback indicator
        if (_currentAudioClip != null && _currentAudioClip.length > 0)
        {
            float normalizedTime = _playbackTime / _currentAudioClip.length;
            float indicatorX = waveformRect.x + normalizedTime * waveformRect.width;

            Handles.color = Color.red;
            Handles.DrawLine(new Vector2(indicatorX, waveformRect.y), new Vector2(indicatorX, waveformRect.yMax));
            Handles.color = Color.white; // Reset color
        }

        // Allow scrubbing by clicking on the waveform
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseDown && waveformRect.Contains(currentEvent.mousePosition))
        {
            if (currentEvent.button == 0) // Left mouse button
            {
                float clickX = currentEvent.mousePosition.x - waveformRect.x;
                float newNormalizedTime = clickX / waveformRect.width;
                _playbackTime = newNormalizedTime * _currentAudioClip.length;
                if (_audioSource != null)
                {
                    _audioSource.time = _playbackTime;
                }
                Repaint();
                currentEvent.Use(); // Consume the event
            }
        }
    }

    // Draws the audio playback controls.
    private void DrawPlaybackControls()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(_isPlaying ? "Pause" : "Play"))
        {
            TogglePlayPause();
        }
        if (GUILayout.Button("Stop"))
        {
            StopAudio();
        }

        // Playback time slider
        EditorGUI.BeginChangeCheck();
        _playbackTime = EditorGUILayout.Slider(_playbackTime, 0f, _currentAudioClip.length);
        if (EditorGUI.EndChangeCheck())
        {
            if (_audioSource != null)
            {
                _audioSource.time = _playbackTime;
            }
            Repaint();
        }

        // Display current time / total duration
        EditorGUILayout.LabelField($"{_playbackTime:F2}s / {_currentAudioClip.length:F2}s", GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();
    }

    // Draws the list of transcription entries.
    private void DrawTranscriptionEntries()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Transcription Entries", EditorStyles.boldLabel);

        // Button to add a new entry at the current playback time
        if (GUILayout.Button("Add Entry at Current Time"))
        {
            AddNewTranscriptionEntry(_playbackTime);
        }

        EditorGUILayout.Space();

        // Scrollable view for entries
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(Mathf.Max(100, position.height - _waveformHeight - 150)));

        // Iterate through the serialized property for proper Undo/Redo and saving
        for (int i = 0; i < _transcriptionEntriesProperty.arraySize; i++)
        {
            SerializedProperty entryProperty = _transcriptionEntriesProperty.GetArrayElementAtIndex(i);
            SerializedProperty timestampProp = entryProperty.FindPropertyRelative("timestamp");
            SerializedProperty laneProp = entryProperty.FindPropertyRelative("lane");

            EditorGUILayout.BeginHorizontal(GUI.skin.box); // Use a box style for each entry
            EditorGUILayout.LabelField($"{timestampProp.floatValue:F2}s", GUILayout.Width(60));

            // Button to jump to this timestamp
            if (GUILayout.Button("▶", GUILayout.Width(25)))
            {
                JumpToTime(timestampProp.floatValue);
            }

            // Text field for the entry
            EditorGUI.BeginChangeCheck();
            laneProp.intValue = EditorGUILayout.IntField(laneProp.intValue, GUILayout.ExpandWidth(true), GUILayout.MinHeight(20));
            if (EditorGUI.EndChangeCheck())
            {
                // Mark the object dirty if text changes
                EditorUtility.SetDirty(_currentTranscriptionData);
            }

            // Button to remove this entry
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                RemoveTranscriptionEntry(i);
                EditorGUILayout.EndHorizontal();
                // Break after removing to avoid issues with array size changing mid-loop
                // The Repaint() call will redraw the correct list in the next OnGUI cycle.
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    // Initializes the AudioSource for playback in the editor.
    private void InitializeAudioSource()
    {
        if (_audioSource == null && _currentAudioClip != null)
        {
            // Create a temporary GameObject to hold the AudioSource in the scene.
            // This GameObject will be hidden and temporary.
            _audioSourceGO = new GameObject("EditorAudioSource");
            _audioSourceGO.hideFlags = HideFlags.HideAndDontSave; // Don't save it with the scene
            _audioSource = _audioSourceGO.AddComponent<AudioSource>();
            _audioSource.clip = _currentAudioClip;
            _audioSource.playOnAwake = false;
        }
    }

    // Cleans up the temporary AudioSource GameObject.
    private void CleanupAudioSource()
    {
        if (_audioSourceGO != null)
        {
            DestroyImmediate(_audioSourceGO);
            _audioSourceGO = null;
            _audioSource = null;
        }
    }

    // Toggles play/pause state of the audio.
    private void TogglePlayPause()
    {
        if (_audioSource == null)
        {
            InitializeAudioSource();
            if (_audioSource == null) return; // Still null, cannot play
        }

        if (_isPlaying)
        {
            _audioSource.Pause();
            _isPlaying = false;
        }
        else
        {
            _audioSource.Play();
            _isPlaying = true;
        }
        Repaint();
    }

    // Stops the audio playback.
    private void StopAudio()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.time = 0f;
            _playbackTime = 0f;
        }
        _isPlaying = false;
        Repaint();
    }

    // Jumps the audio playback to a specific time.
    private void JumpToTime(float time)
    {
        if (_audioSource != null)
        {
            _audioSource.time = time;
            _playbackTime = time;
            if (!_isPlaying) // If not playing, start playing from here
            {
                _audioSource.Play();
                _isPlaying = true;
            }
        }
        Repaint();
    }

    // Adds a new transcription entry at the given time.
    private void AddNewTranscriptionEntry(float time)
    {
        // Ensure the serialized object is up-to-date with any previous changes
        _serializedObject.Update();

        // Add a new element to the serialized property array
        _transcriptionEntriesProperty.InsertArrayElementAtIndex(_transcriptionEntriesProperty.arraySize);
        SerializedProperty newEntry = _transcriptionEntriesProperty.GetArrayElementAtIndex(_transcriptionEntriesProperty.arraySize - 1);

        // Set the values for the new entry
        newEntry.FindPropertyRelative("timestamp").floatValue = time;
        newEntry.FindPropertyRelative("lane").intValue = Random.Range(0, 4); 

        // Apply these changes to the underlying ScriptableObject immediately
        _serializedObject.ApplyModifiedProperties();

        // Now that the underlying list is updated, sort it.
        // SortTranscriptionEntries will also call EditorUtility.SetDirty and Repaint.
        SortTranscriptionEntries();
    }

    // Removes a transcription entry at the specified index.
    private void RemoveTranscriptionEntry(int index)
    {
        // Ensure the serialized object is up-to-date before modifying
        _serializedObject.Update();
        _transcriptionEntriesProperty.DeleteArrayElementAtIndex(index);
        // Apply changes to the underlying ScriptableObject immediately
        _serializedObject.ApplyModifiedProperties();

        // Mark the ScriptableObject as dirty so changes are saved.
        EditorUtility.SetDirty(_currentTranscriptionData);
        Repaint();
    }

    // Sorts the transcription entries by timestamp.
    private void SortTranscriptionEntries()
    {
        // Direct modification of the list in the ScriptableObject.
        // This is safe because we're doing it within an editor script
        // and immediately marking the object dirty.
        _currentTranscriptionData.transcriptionEntries = _currentTranscriptionData.transcriptionEntries
            .OrderBy(entry => entry.timestamp)
            .ToList();

        // Re-synchronize the SerializedObject with the directly modified underlying object.
        // This is crucial because direct list modification breaks SerializedProperty's internal indexing.
        _serializedObject.Update();

        // Mark the ScriptableObject as dirty so changes are saved.
        EditorUtility.SetDirty(_currentTranscriptionData);
        Repaint();
    }

    // This Update method is called continuously when the editor window is open.
    private void Update()
    {
        if (_isPlaying && _audioSource != null && _audioSource.isPlaying)
        {
            // Update playback time and repaint the window to show progress.
            _playbackTime = _audioSource.time;
            Repaint();
        }
        else if (_isPlaying && _audioSource != null && !_audioSource.isPlaying)
        {
            // If audio stopped playing (e.g., reached end), reset state.
            _isPlaying = false;
            _playbackTime = _currentAudioClip.length; // Ensure it shows end time
            Repaint();
        }

        // If the window size changes, regenerate the waveform texture
        // (This is a simple way; for complex apps, optimize this).
        if (_waveformTexture != null && _waveformTexture.width != (int)position.width)
        {
            GenerateWaveformTexture();
            Repaint();
        }
    }
}