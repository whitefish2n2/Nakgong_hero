using System;
using System.Collections;
using UnityEngine;

namespace Source
{
    public class GameUtil : MonoBehaviour
    {
        public static GameUtil instance;
        void Start()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void CoolBool(float t, Action<bool> callback, bool to = true)
        {
            StartCoroutine(CoolBoolIE(t, callback, to));
        }
        private IEnumerator CoolBoolIE(float t, Action<bool> target, bool to = true)
        {
            target(!to);
            yield return new WaitForSeconds(t);
            target(to);
        }
    }
}
