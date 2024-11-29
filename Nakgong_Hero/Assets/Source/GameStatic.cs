using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source
{
    public class GameStatic : MonoBehaviour
    {
        public static GameStatic instance;
        public GameObject hpCanvas;
        [HideInInspector] public Camera mainCam;
        public GameObject damagePrefab;
        public GameObject hpBarPrefabMini;
        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            mainCam = Camera.main;
        }

        private void Start()
        {
            Debug.Log(hpCanvas.name);
        }
    }
}
