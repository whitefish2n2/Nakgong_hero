using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndButtonManager : MonoBehaviour
{
    private void Start()
    {
        GameObject[] objs = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in objs)
        {
            if (obj.scene.name == null) // DontDestroyOnLoad 객체는 씬 이름이 null임
            {
                Destroy(obj);
            }
        }
    }

    public void ReStart()
    {
        SceneManager.LoadScene("Main_Insert");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
