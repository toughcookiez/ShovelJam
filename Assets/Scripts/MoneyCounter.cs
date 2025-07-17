using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private BandStats _bandStats;

    public float incrementSpeed = 1.0f; // How many integer points to add per second

    private bool _updatingText = false;


    private void Start()
    {
        if (_bandStats.UImoneyAmount == _bandStats.Money)
        {
            _textMeshPro.text = _bandStats.UImoneyAmount.ToString();
        }
    }

    private void UpdateText()
    {
        if (!_updatingText && _textMeshPro.text != _bandStats.Money.ToString())
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

                _textMeshPro.text = Math.Ceiling(Mathf.Lerp(currentValue, _bandStats.Money, Mathf.Clamp01(elpasedTime / totalTime))).ToString();
                yield return null;
            }
            while (elpasedTime <= totalTime);

            _bandStats.UImoneyAmount = _bandStats.Money;
        }
        finally
        {
            _updatingText=false;
        }
    }

}
