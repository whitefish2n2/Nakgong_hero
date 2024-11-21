using System.Collections;
using System.Collections.Generic;
using Source;
using Source.GameManager;
using Source.PlayerCode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
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

    public void LoadStartScene(float d = 2)
    {
        PlayerController.instance.BeAttackAble();
        SceneLoader.Instance.LoadScene("Stage1",d,new Vector2(-14f,126f), refreshPlayerStates:true);
    }

    public void StartAction()
    {
        wallForClose.SetActive(true);
    }
    
}
