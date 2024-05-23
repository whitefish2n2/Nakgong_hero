using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [SerializeField] private Transform Parent;
    [SerializeField] private Transform Deager;
    [SerializeField] float mode;
    private HingeJoint2D HingeJoint2D;
    void Start()
    {
        HingeJoint2D = gameObject.GetComponent<HingeJoint2D>();
    }
    void FixedUpdate()
    {
        if (mode == 2)
        {
            gameObject.transform.position = Deager.position;
        }
        if (mode == 1)
        {
            gameObject.transform.position =
                new Vector3(PlayerController.PlayerPos.x-0.099f, PlayerController.PlayerPos.y - 0.809f); 
        }
    }
}
