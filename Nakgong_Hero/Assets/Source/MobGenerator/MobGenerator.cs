using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] GenList;
    [Header("생성 위치(상대적)")]
    [SerializeField] private Vector2[] GenPos;

    public void Trigger()
    {
        for (int i = 0; i < GenList.Length; i++)
        {
            GameObject Instance = Instantiate(GenList[i], (Vector2)transform.position + GenPos[i], Quaternion.identity);
        }
    }
}
