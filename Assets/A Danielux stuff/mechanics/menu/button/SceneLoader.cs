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
    public GameObject fade;
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

        yield return new WaitForSeconds(fade.GetComponent<Animator>().runtimeAnimatorController.animationClips.Length);
        SceneManager.LoadScene(Index);

    }

    public int GetCurrentLevelIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 3;
    }

    public void ChangeScene(int SceneIndex)
    {
        StartCoroutine(Fade(SceneIndex));
    }
}
