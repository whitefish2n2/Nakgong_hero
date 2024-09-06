using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
    
}
