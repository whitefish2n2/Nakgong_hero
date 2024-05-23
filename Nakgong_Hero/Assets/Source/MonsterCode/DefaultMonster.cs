using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultMonster : MonoBehaviour
{
    [SerializeField]private float HP;
    private float HP_Temp;
    [SerializeField] private float stans; 
    [SerializeField] private float KnockbackForce;
    private Rigidbody2D thisRigidbody2D;
    private float stanstemp;
    [Header("HP_BAR")]
    [SerializeField] private GameObject leftHP_Bar;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private float distance;
    private GameObject LeftbarInstance;
    private RectTransform HpBar;

    void Awake()
    {
        LeftbarInstance = Instantiate(leftHP_Bar, Canvas.transform);
        HpBar = LeftbarInstance.GetComponent<RectTransform>();
        thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        LeftbarInstance.gameObject.GetComponent<Image>().color = new Color32(40, 140, 0,255);
        stanstemp = stans;
        HP_Temp = HP;
    }
    private void Update()
    {
        HpBar.position =
            Camera.main.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + distance));
    }
    public void gotattack(string AttackMode, float Damage, float stansMinus)
    {
        if (AttackMode == "Default")
        {
            Debug.Log("aya ");
            HP -= Damage + PlayerController.AirBonePower/100f;
            HPUpdate();
            stans -= stansMinus;
            if (stans <= 0f)
            {
                KnockBack();
            }
        }
    }

    private void KnockBack()
    {
        
        if (gameObject.transform.position.x - PlayerController.PlayerPos.x > 0)
        {
            thisRigidbody2D.AddForce(new Vector2(1f*KnockbackForce - stans,PlayerController.AirBonePower));
        }
        else
        {
            thisRigidbody2D.AddForce(new Vector2(-1f*KnockbackForce,PlayerController.AirBonePower));
        }
        stans = stanstemp;
    }
    private void HPUpdate()
    {
        LeftbarInstance.GetComponent<Image>().fillAmount = HP / HP_Temp;
        if (HP / HP_Temp < 0.3f)
        {
            LeftbarInstance.gameObject.GetComponent<Image>().color = new Color32(180, 0, 0,255);
        }
        else if (HP / HP_Temp < 0.7f)
        {
            LeftbarInstance.gameObject.GetComponent<Image>().color = new Color32(225, 80, 0,255);
        }
        else
        {
            LeftbarInstance.gameObject.GetComponent<Image>().color = new Color32(20, 140, 0,255);
        }
        if (HP <= 0f)
        {
            Dead();
        }
    }

    private void Dead()
    {
        Debug.Log("으앙죽음ㅜㅜ");
        Destroy(gameObject);
    }
}
