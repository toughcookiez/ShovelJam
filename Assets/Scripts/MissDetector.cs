using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissDetector : MonoBehaviour
{
    public int _mistakes { get; set; }

    [SerializeField] private GameObject NoteHolder;

    [SerializeField] private int _mistakeLimit = 5;

    [SerializeField] private AudioSource _musicSource;

    [SerializeField] private AudioClip _cheerClip;

    [SerializeField] private int _mistakeLimitForLilBonus;

    [SerializeField] private int _mistakeLimitForBigBonus;

    [SerializeField] private BandStats _bandStats;

    [SerializeField] private int _moneyRewardForLilBonus;

    [SerializeField] private int _moneyRewardForBigBonus;

    [SerializeField] private int _fanRewardForLilBonus;

    [SerializeField] private int _fanRewardForBigBonus;

    [SerializeField] private int _moneyPenaltyForLosing;

    [SerializeField] private int _fanPenaltyForLosing;

    [SerializeField] private SpriteRenderer[] keys;

    private int _moneyRewardForStartingLevel;

    private AudioSource _audioSource;

    private bool _isDone;


    public int _currentMoneyReward { get; set; }

    


    

    



    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _mistakeLimitForLilBonus = _bandStats.GetNumberOfAllowedMistakesForBonus(_mistakeLimitForLilBonus);
        _mistakeLimitForBigBonus = _bandStats.GetNumberOfAllowedMistakesForBonus(_mistakeLimitForBigBonus);

        _bandStats.ApplyRemoveCards();

        
        _moneyRewardForStartingLevel = SceneLoader.Instance._startPayments[SceneManager.GetActiveScene().buildIndex - 3];
        _currentMoneyReward = _moneyRewardForStartingLevel + _moneyRewardForBigBonus;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("note"))
        {

            
            Destroy(collision.gameObject);
            _mistakes++;
            if (_bandStats.ReviseMistakeLimit(_mistakes) >= _mistakeLimit)
            {
                StartCoroutine(LoseLevel());
            }
            StartCoroutine(ChangeKeyColor());
        }


        if (_mistakes > _mistakeLimitForBigBonus && _mistakes < _mistakeLimitForLilBonus)
        {
            _currentMoneyReward = _moneyRewardForStartingLevel + _mistakeLimitForLilBonus;
        }
        else if (_mistakes > _mistakeLimitForLilBonus && _mistakes < _mistakeLimit)
        {
            _currentMoneyReward = _moneyRewardForStartingLevel;
        }
        else if (_mistakes > _mistakeLimit)
        {
            _currentMoneyReward = _moneyPenaltyForLosing;
        }
    }

    private void Update()
    {
        if (_musicSource != null && !_musicSource.isPlaying && _mistakes < _mistakeLimit) 
        {
            if (_mistakes < _mistakeLimitForLilBonus && _mistakes > _mistakeLimitForBigBonus)
            {
               _bandStats.UpdateEarnedMoney(_moneyRewardForLilBonus);
                _bandStats.UpdateFans(_fanRewardForLilBonus);
            }
            else if (_mistakes > _mistakeLimitForBigBonus)
            {
               _bandStats.UpdateEarnedMoney(_moneyRewardForBigBonus);
                _bandStats.UpdateFans(_fanRewardForBigBonus);
            }

            _bandStats.ApplyRemoveCards();

            StartCoroutine(WinLevel());
        }
    }

    private IEnumerator ChangeKeyColor()
    {
        Color color0 = keys[0].color;
        Color color1 = keys[1].color;
        Color color2 = keys[2].color;
        Color color3 = keys[3].color;
        keys[0].color = Color.red;
        keys[1].color = Color.red;
        keys[2].color = Color.red;
        keys[3].color = Color.red;
        yield return new WaitForSeconds(.1f);
        keys[0].color = color0;
        keys[1].color = color1;
        keys[2].color = color2;
        keys[3].color = color3;
    }

    private IEnumerator LoseLevel()
    {
        NoteHolder.SetActive(false);
        _bandStats.UpdateEarnedMoney(-_moneyPenaltyForLosing);
        _bandStats.ApplyRemoveCards();
        while (_musicSource.volume != 0)
        {
            _musicSource.volume -= .01f;
            yield return null;
        }
        _audioSource.Play();
        yield return new WaitForSeconds(2f);
        while (_audioSource.volume != 0)
        {
            _audioSource.volume -= .005f;
            if ( _audioSource.volume < .5f )
            {
                SceneLoader.Instance.ChangeScene(1);
            }
            yield return null;
        }
        

    }

    private IEnumerator WinLevel()
    {
            SceneLoader.Instance.OpenNextLevel();
        _audioSource.clip = _cheerClip;
        _audioSource.Play();
        yield return new WaitForSeconds(2f);
        while (_audioSource.volume != 0)
        {
            _audioSource.volume -= .005f;
            if (_audioSource.volume < .5f)
            {
                SceneLoader.Instance.ChangeScene(2);
            }
            yield return null;
        }


    }
}
