using System.Collections;
using System.Collections.Generic;
using Source.PlayerCode;
using UnityEngine;
using UnityEngine.Events;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private UnityEvent OnEnd;

    public void MoveStart(Vector2 to, float time, bool stopPlayer)
    {
        StartCoroutine(Move(transform.position, to, time, stopPlayer));
    }
    private void MoveEnd()
    {
        OnEnd?.Invoke();
    }

    IEnumerator Move(Vector2 from, Vector2 to, float time, bool stopPlayer)
    {
        var elapsedTime = 0f;
        var tagTemp = transform.tag;
        gameObject.transform.tag = "Moving";
        if(stopPlayer)
            PlayerController.Instance.Stop();
        while (elapsedTime < time)
        {
            gameObject.transform.position = Vector2.Lerp(from, to, elapsedTime / time);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        gameObject.transform.position = to;
        gameObject.transform.tag = tagTemp;
        if (stopPlayer)
            PlayerController.Instance.DisStop();
        MoveEnd();
    }
}
