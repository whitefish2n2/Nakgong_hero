using System;
using System.Collections;
using System.Collections.Generic;
using Source.MapSource.MapMoveManager;
using UnityEngine;
using UnityEngine.Serialization;

public class Wall : MonoBehaviour
{
    [Header("상대적 움직임.")] [SerializeField] private Vector2 moveTo;
    [SerializeField] private float moveTime;
    [SerializeField] private bool stopPlayer;
    private Vector2 startPosition;
    private bool moved = false;
    private void Awake()
    {
        startPosition = transform.position;
        moveTo = new Vector2(startPosition.x + moveTo.x, startPosition.y + moveTo.y);
    }

    public void WallMove()
    {
        if (moved)
        {
            gameObject.GetComponent<MovingObject>().MoveStart(startPosition, moveTime,stopPlayer);
            moved = false;
        }
        else
        {
            gameObject.GetComponent<MovingObject>().MoveStart(moveTo, moveTime, stopPlayer);
            moved = true;
        }
        
    }
}
