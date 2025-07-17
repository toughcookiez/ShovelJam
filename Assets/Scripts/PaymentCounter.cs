using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PaymentCounter : MonoBehaviour
{

    [SerializeField] private MissDetector _missDetector;

    [SerializeField] private TextMeshProUGUI _rewardText;

    [SerializeField] private TextMeshProUGUI _mistakeText;

    [SerializeField] private bool _updatingText = false;

    private void Start()
    {
    }

    private void UpdateText()
    {
        if (!_updatingText && _rewardText.text != _missDetector._currentMoneyReward.ToString())
        {
            StartCoroutine(ChangeRewardText());
        }
        if (!_updatingText && _mistakeText.text != _missDetector._mistakes.ToString())
        {
            StartCoroutine(ChangeMistakeText());
        }

    }

    private void Update()
    {
        UpdateText();
    }

    IEnumerator ChangeRewardText()
    {
        try
        {
            _updatingText = true;

            int currentValue = int.Parse(_rewardText.text);


            float elpasedTime = 0;
            float totalTime = 2;

            do
            {
                elpasedTime += Time.deltaTime;
                _rewardText.text = Math.Ceiling(Mathf.Lerp(currentValue, _missDetector._currentMoneyReward, Mathf.Clamp01(elpasedTime / totalTime))).ToString();
                yield return null;
            }
            while (elpasedTime <= totalTime);
        }
        finally
        {
            _updatingText = false;
        }
    }

    IEnumerator ChangeMistakeText()
    {
        try
        {
            _updatingText = true;

            int currentValue = int.Parse(_mistakeText.text);


            float elpasedTime = 0;
            float totalTime = 2;

            do
            {
                elpasedTime += Time.deltaTime;
                _mistakeText.text = Math.Ceiling(Mathf.Lerp(currentValue, _missDetector._mistakes, Mathf.Clamp01(elpasedTime / totalTime))).ToString();
                yield return null;
            }
            while (elpasedTime <= totalTime);
        }
        finally
        {
            _updatingText = false;
        }
    }
}
