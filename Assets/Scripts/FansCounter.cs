using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FansCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private BandStats _bandStats;

    private bool _updatingText = false;

    private void Start()
    {
        if (_bandStats.UIfanAmount == _bandStats.Fans)
        {
            _textMeshPro.text = _bandStats.UIfanAmount.ToString();
        }
    }

    private void UpdateText()
    {
        if (!_updatingText && _textMeshPro.text != _bandStats.Fans.ToString())
        {

            StartCoroutine(ChangeText());
        }
    }

    private void Update()
    {
        UpdateText();
    }

    IEnumerator ChangeText()
    {
        try
        {
            _updatingText = true;

            int currentValue = int.Parse(_textMeshPro.text);


            float elpasedTime = 0;
            float totalTime = 2;

            do
            {
                elpasedTime += Time.deltaTime;

                _textMeshPro.text = Math.Ceiling(Mathf.Lerp(currentValue, _bandStats.Fans, Mathf.Clamp01(elpasedTime / totalTime))).ToString();
                yield return null;
            }
            while (elpasedTime <= totalTime);

            _bandStats.UIfanAmount = _bandStats.Fans;
        }
        finally
        {
            _updatingText = false;
        }
    }
}
