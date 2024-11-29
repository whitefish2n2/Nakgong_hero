using System;
using Source.GameManager;
using Source.PlayerCode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Stages
{
    public class StageManager : MonoBehaviour
    {
        [Header("다음 씬 설정")]
        public string nextScene;
        public float nextSceneLoadDelay;
        public bool changePos = true;
        public Vector2 nextSceneStartPosition;
        public bool reFreshPlayer;
        public float nextStartFadeTime = 0.5f;
        public float nextEndFadeTime = 0.5f;
        public static StageManager instance;
        
        public virtual void SetNextScene(string sceneName)
        {
            this.nextScene = sceneName;
        }

        public virtual void NextScene()
        {
            SceneLoader.Instance.LoadScene(nextScene,nextSceneLoadDelay, changePos ?nextSceneStartPosition :
                PlayerController.instance.playerPos,reFreshPlayer,nextStartFadeTime,nextEndFadeTime);
        }
        /// <summary>
        /// 이런저런 효과 적용시키는 맵일때 죽으면 초기화 시켜주기 위함
        /// </summary>
        public virtual void PlayerDeadOnThisScene()
        {
            throw new System.NotImplementedException();
        }
    }
}
