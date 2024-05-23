using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDefaultMove : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float smooth;
    private Transform P_Transform;
    private Vector3 PlayerPosition;
    public static float CameraposPlus;
    private void Start()
    {
        P_Transform = player.GetComponent<Transform>();
    }

    private void Update()
    {
        PlayerPosition = new Vector3(P_Transform.position.x, P_Transform.position.y+CameraposPlus, transform.position.z);
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Lerp(PlayerPosition.x, transform.position.x, smooth),
            Mathf.Lerp(PlayerPosition.y, transform.position.y, smooth), transform.position.z);

    }
    
}
