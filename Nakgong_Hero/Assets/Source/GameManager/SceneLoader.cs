using System;
using System.Collections;
using System.Collections.Generic;
using Source.PlayerCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Source.GameManager
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;
        [SerializeField] private GameObject loadPanel;
        private CanvasRenderer _panelRenderer;
        private GameObject _loadFade;
        private GameObject _loadBar;
        private Image _loadBarImage;
        private GameObject _loadAnim;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(loadPanel);
            _loadBar = loadPanel.transform.GetChild(0).gameObject;
            _loadBar = loadPanel.transform.GetChild(0).GetChild(0).gameObject;
            _loadAnim = loadPanel.transform.GetChild(0).GetChild(1).gameObject;
            _panelRenderer = loadPanel.GetComponent<CanvasRenderer>();
            _loadBarImage = _loadBar.GetComponent<Image>();
        }

        public void LoadScene(string sceneName, float delay = 0, Vector2 newPlayerPos = default)
        {
            SceneManager.LoadScene("Load");
            StartCoroutine(LoadSceneIE(sceneName, newPlayerPos, delay));
        }

        private IEnumerator LoadSceneIE(string sceneName, Vector2 newPlayerPos, float delay, float startFade = 0.5f, float endFade = 0.5f)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            if (asyncOperation != null) asyncOperation.allowSceneActivation = false;
            else throw new Exception("Scene not found");
            //fade in
            loadPanel.SetActive(true);
            float elapsedTime = 0;
            while (elapsedTime < startFade)
            {
                elapsedTime += Time.deltaTime;
                _panelRenderer.SetAlpha(elapsedTime / startFade);
                yield return null;
            }
            //씬 이동
            while (!asyncOperation.isDone)
            {
                _loadBarImage.fillAmount = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(delay);
                    asyncOperation.allowSceneActivation = true;
                    PlayerController.Instance.gameObject.transform.position = newPlayerPos;
                }
            }
            //fade out
            elapsedTime = 0;
            while (elapsedTime < startFade)
            {
                elapsedTime += Time.deltaTime;
                _panelRenderer.SetAlpha(startFade - elapsedTime / startFade);
                yield return null;
            }
            loadPanel.SetActive(false);
        }
    }
}
