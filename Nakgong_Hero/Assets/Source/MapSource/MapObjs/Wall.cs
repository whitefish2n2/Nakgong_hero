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
        var position = transform.position;
        startPosition = position;
        moveTo = new Vector2(position.x + moveTo.x, position.y + moveTo.y);
    }

    public void WallMove()
    {
        if (moved)
        {
            gameObject.GetComponent<MovingObject>().MoveStart(startPosition, moveTime,true);
            moved = false;
        }
        else
        {
            gameObject.GetComponent<MovingObject>().MoveStart(moveTo, moveTime, true);
            moved = true;
        }
        
    }
}
