using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private GameObject Canvas;
    [SerializeField] private float distance;
    private GameObject LeftbarInstance;
    private RectTransform HpBar;

    void Start()
    {
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
        LeftbarInstance = Instantiate(leftHP_Bar, Canvas.transform);
        HpBar = LeftbarInstance.GetComponent<RectTransform>();
        thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        LeftbarInstance.gameObject.GetComponent<Image>().color = new Color32(40, 140, 0,255);
        HP *= InvManager.Instance.Difficulty;
        stans *= InvManager.Instance.Difficulty;
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
        float RealDamage = Damage + InvManager.Instance.AirBonePower/100f;
        if (AttackMode == "Default")
        {
            GameObject instance = Instantiate(DamagePrefabManager.DamagePrefab,
                Canvas.transform);
            instance.transform.position =
                Camera.main.WorldToScreenPoint((Vector2)gameObject.transform.position + Random.insideUnitCircle * 0.01f);
            instance.GetComponent<TextMeshProUGUI>().text = ((int)RealDamage).ToString();
            instance.GetComponent<Animator>().Play("DamageOn");
            Destroy(instance,1f);
            HP -= RealDamage;
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
            thisRigidbody2D.AddForce(new Vector2(1f*KnockbackForce - stans,InvManager.Instance.AirBonePower));
        }
        else
        {
            thisRigidbody2D.AddForce(new Vector2(-1f*KnockbackForce,InvManager.Instance.AirBonePower));
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
