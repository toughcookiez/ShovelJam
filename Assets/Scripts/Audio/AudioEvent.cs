using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    [System.Serializable]
    public class AudioEvent : UnityEvent<int> { }

}