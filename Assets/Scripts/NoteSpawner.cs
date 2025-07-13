using Assets.Scripts;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject NotePrefab;

    private int _lastNote = -1;
    private int _nextNote = -1;

    public void Update()
    {
        if (_lastNote !=  _nextNote)
        {
            _lastNote = _nextNote;
            Instantiate(NotePrefab, transform.position + (Vector3.left * _lastNote  * 5), Quaternion.identity);
        }
    }

    public void SpawnNote(int noteIndex)
    {
        _nextNote = noteIndex % 4;
    }
}
