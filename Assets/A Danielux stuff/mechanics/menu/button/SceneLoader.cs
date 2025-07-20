using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance
    {
        get;
        private set;
    }

    public List<Level> Levels;

    [SerializeField] public List<int> _startPayments;
    [SerializeField] public BandStats _bandStats;
    [SerializeField] public AudioSource _audioSource;
    public GameObject fade;

    private bool _fadingMusic;
    [Tooltip("the scene you want the player to go")]
    public int SceneTP;

    

    private void Start()
    {
        Instance = this;
        fade.SetActive(true);
    }

    public void LoadSong(int sceneIndex)
    {
        
        if (!Levels[sceneIndex].isOpen)
        {
            return;
        }

        if (_bandStats.hasCard(c => c is MoneyCard))
        {
            _bandStats.Money += _startPayments[sceneIndex] + 200;
        }
        else
        {
            _bandStats.Money += _startPayments[sceneIndex];
        }
        StartCoroutine(Fade(sceneIndex + 3));
    }

    public void Click()
    {
        if (_bandStats.hasCard(c => c is MoneyCard))
        {
            _bandStats.Money += _startPayments[SceneTP] + 200;
        }
        else
        {
            _bandStats.Money += _startPayments[SceneTP];
        }
        StartCoroutine(Fade(SceneTP));
    }
    IEnumerator Fade(int Index)
    {
        fade.GetComponent<Animator>().SetBool("activate", true);
        _fadingMusic = true;
        yield return new WaitForSeconds(fade.GetComponent<Animator>().runtimeAnimatorController.animationClips.Length);
        SceneManager.LoadScene(Index);

    }

    private void Update()
    {
        if (_fadingMusic && _audioSource != null)
        {
            if (_audioSource.volume > 0)
            {
                _audioSource.volume -= .01f;
            }
        }
    }

    public int GetCurrentLevelIndex()
    {
        return SceneManager.GetActiveScene().buildIndex - 3;
    }

    public void ChangeScene(int SceneIndex)
    {
        StartCoroutine(Fade(SceneIndex));
    }

    internal void OpenNextLevel()
    {
        if (Levels[GetCurrentLevelIndex() + 1] != null)
        {
            Levels[GetCurrentLevelIndex() + 1].isOpen = true;
        }
    }
}
