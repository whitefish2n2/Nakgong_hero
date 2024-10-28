using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInteract : MonoBehaviour
{
    public static ItemInteract Instance;
    private TMP_Text TextPanel;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        TextPanel = gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ChangeText(string Text)
    {
        TextPanel.text = Text;
    }

    public void InteractOnHere(Vector3 Position)
    {
        transform.position = _camera.WorldToScreenPoint(Position);
        gameObject.SetActive(true);
    }

    public void InteractOut()
    {
        gameObject.SetActive(false);
    }
}
