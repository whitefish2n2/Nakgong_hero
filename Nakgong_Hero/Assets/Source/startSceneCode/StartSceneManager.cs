using Scenes.Stages;
using Source.GameManager;
using Source.PlayerCode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Source.startSceneCode
{
    public class StartSceneManager : StageManager
    {
        [SerializeField] private GameObject baseObject;
        [SerializeField] private GameObject[] titles;
        [SerializeField] private GameObject wallForClose;
        public void PressAnyButton()
        {
            if (Input.GetKey(KeyCode.D))
            {
                PlayerController.instance.DisStop();
                LoadStartScene(0);
                return;
            }
            baseObject.GetComponent<PlayableDirector>().Play();
            foreach(var o in titles)
            {
                o.SetActive(false);
            }
        }

        public override void NextScene()
        {
            PlayerController.instance.BeAttackAble();
            base.NextScene();
        }

        public void LoadStartScene(float d = 2)
        {
            SceneLoader.Instance.LoadScene("Stage1",d,new Vector2(-14f,126f), refreshPlayerStates:true);
        }

        public void StartAction()
        {
            wallForClose.SetActive(true);
        }
    
    }
}
