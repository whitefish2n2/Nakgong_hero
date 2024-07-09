using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Wall : MonoBehaviour
{
    [Header("상대적 움직임.")] [SerializeField] private Vector2 moveTo;
    [SerializeField] private float moveTime;
    private Vector2 startPosition;
    private bool moved = false;
    private void Awake()
    {
        startPosition = transform.position;
        moveTo += startPosition;
    }

    public void WallMove()
    {
        gameObject.GetComponent<MovingObject>().MoveStart((moved = !moved) ? moveTo : startPosition, moveTime,true);
    }
}
