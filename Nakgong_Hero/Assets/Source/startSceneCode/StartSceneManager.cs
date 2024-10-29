using System.Collections;
using System.Collections.Generic;
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
        baseObject.GetComponent<PlayableDirector>().Play();
        
        foreach(var o in titles)
        {
            o.SetActive(false);
        }
    }

    public void LoadStartScene()
    {
        PlayerController.Instance.BeAttackAble();
        PlayerController.Instance.LoadScene("Stage1",new Vector2(-14f,126f));
    }
}
