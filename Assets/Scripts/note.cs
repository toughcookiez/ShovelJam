using System;
using System.Collections;
using UnityEngine;

public class note : MonoBehaviour
{
    public Transform _targetTransform {  get; set; }

    [SerializeField]
    private float _duration;


    private void Start()
    {
        StartCoroutine(Fall(_duration));
    }

    IEnumerator Fall(float duration)
    {
        if (duration <= 0)
        {
            Debug.LogError("Duration is invalid");

        }
        else
        {
            float elpasedTime = 0;
            float startY = transform.position.y;

            while (elpasedTime < duration)
            {

                elpasedTime += Time.deltaTime;

                transform.position = new Vector3(transform.position.x, Mathf.Lerp(startY, _targetTransform.position.y, elpasedTime / duration), transform.position.z);

                yield return null;
            }

            float targetY2 = _targetTransform.position.y - Math.Abs(_targetTransform.position.y - startY);

            elpasedTime = 0;
            while (elpasedTime < duration)
            {
                elpasedTime += Time.deltaTime;

                transform.position = new Vector3(transform.position.x, Mathf.Lerp(_targetTransform.position.y, targetY2, elpasedTime / duration), transform.position.z);



                yield return null;
            }

        }

    }


}
