using System.Collections;
using System.Collections.Generic;
using Source.GameManager;
using Source.PlayerCode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject baseObject;
    [SerializeField] private GameObject[] titles;
    public void pressAnyButton()
    {
        if (Input.GetKey(KeyCode.D))
        {
            PlayerController.Instance.DisStop();
            LoadStartScene();
            return;
        }
        baseObject.GetComponent<PlayableDirector>().Play();
        foreach(var o in titles)
        {
            o.SetActive(false);
        }
    }

    public void LoadStartScene()
    {
        PlayerController.Instance.BeAttackAble();
        SceneLoader.Instance.LoadScene("Stage1",2,new Vector2(-14f,126f));
    }
    
}
