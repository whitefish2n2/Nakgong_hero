using System;
using System.Collections;
using System.Collections.Generic;
using Source.PlayerCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Source.GameManager
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;
        [SerializeField] private GameObject loadFade;
        private CanvasRenderer _fadeRenderer;
        private GameObject _loadBar;
        private CanvasRenderer _barRenderer;
        private Image _loadBarImage;
        private CanvasRenderer _loadAnimRenderer;
        private GameObject _loadAnim;
        private bool _isLoading;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(loadFade.transform.parent.gameObject);
            _loadBar = loadFade.transform.GetChild(0).gameObject;
            _loadAnim = loadFade.transform.GetChild(1).gameObject;
            _fadeRenderer = loadFade.GetComponent<CanvasRenderer>();
            _barRenderer = _loadBar.GetComponent<CanvasRenderer>();
            _loadAnimRenderer = _loadAnim.GetComponent<CanvasRenderer>();
            _loadBarImage = _loadBar.GetComponent<Image>();
            _fadeRenderer.SetAlpha(0);
            _loadAnimRenderer.SetAlpha(0);
            _barRenderer.SetAlpha(0);
            _fadeRenderer.gameObject.SetActive(false);
        }

        public void LoadScene(string sceneName, float delay = 0, Vector2 newPlayerPos = default, bool refreshPlayerStates = false, float startFade = 0.5f, float endFade = 0.5f)
        {
            if (_isLoading) return;
            _isLoading = true;
            if(refreshPlayerStates)
                PlayerController.instance.SetNormal();
            StartCoroutine(LoadSceneIE(sceneName, newPlayerPos, delay, startFade, endFade));
        }

        private IEnumerator LoadSceneIE(string sceneName, Vector2 newPlayerPos, float delay, float startFade, float endFade)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            if (asyncOperation != null) asyncOperation.allowSceneActivation = false;
            else throw new Exception("Scene not found");
            //fade in
            loadFade.SetActive(true);
            Time.timeScale = 0;
            float elapsedTime = 0;
            while (elapsedTime < startFade)
            {
                elapsedTime += Time.unscaledDeltaTime;
                var fade = elapsedTime / startFade;
                _fadeRenderer.SetAlpha(fade);
                _loadAnimRenderer.SetAlpha(fade);
                _barRenderer.SetAlpha(fade);
                yield return null;
            }
            PlayerController.instance.Stop();
            //씬 이동
            while (asyncOperation.progress <= 0.41f)
            {
                _loadBarImage.fillAmount = asyncOperation.progress;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(delay);
            while (asyncOperation.progress < 0.9f)
            {
                _loadBarImage.fillAmount = asyncOperation.progress;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(delay);
            asyncOperation.allowSceneActivation = true;
            _loadBarImage.fillAmount = 1;
            yield return new WaitForSecondsRealtime(delay);
            PlayerController.instance.gameObject.transform.position = newPlayerPos;
            //fade out
            elapsedTime = 0;
            PlayerController.instance.DisStop();
            while (elapsedTime < endFade)
            {
                elapsedTime += Time.unscaledDeltaTime;
                var fade = endFade - elapsedTime / endFade;
                _fadeRenderer.SetAlpha(fade);
                _loadAnimRenderer.SetAlpha(fade);
                _barRenderer.SetAlpha(fade);
                yield return null;
            }
            Time.timeScale = 1;
            _fadeRenderer.SetAlpha(0);
            _loadAnimRenderer.SetAlpha(0);
            _barRenderer.SetAlpha(0);
            loadFade.SetActive(false);
            _isLoading = false;
        }
    }
}
