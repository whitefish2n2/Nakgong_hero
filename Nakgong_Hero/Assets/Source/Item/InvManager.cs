using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Unity.Mathematics;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using Random = UnityEngine.Random;

public class InvManager : MonoBehaviour
{
    public static InvManager Instance;
    //플레이어 속도
    public float speed;
    public float startSpeed;
    public float shiftSpeedPlus;
    //플레이어 점프력
    public float jumpPower;
    //플레이어 공격력
    public float AttackPower;
    //플레이어 스택스 파괴율
    public float stans;
    //에어본 세기
    public float AirBonePower;
    //그래비티 스케일
    public float GravityScale;
    public float startGravityScale;
    public float GravityScalePlus;
    //체력
    public float HP;
    public float MaxHP;
    //골드
    public float Gold;
    //난이도
    public float Difficulty;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        AttackPower = 3f;//저장 파일에서 저장된 기본값 받아오자-
        stans = 3f;//몬스터의 스탠스 수치를 얼마나 깎나/기본값
        HP = 50f;
        MaxHP = 100f;
        jumpPower = 5f;
        startSpeed = 150f;
        shiftSpeedPlus = 100f;
        speed = startSpeed;
        startGravityScale = 1f;
        GravityScalePlus = 10f;
        AirBonePower = 0f;
        stans = 100f;
        Difficulty = 1f;
        Instance = this;
    }

    /// <summary>
    /// 커먼 아이템 얻는 함수
    /// </summary>
    /// <param name="CommonItem"></param>
    /// <summary>
    /// 특수 아이템 얻는 함수
    /// </summary>
    /// <param name="특수 아이템"></param>





    public void GetHone()
    {
        AttackPower += 5f;
    }

    public void GetWeight()
    {
        GravityScalePlus += 0.5f;
    }
    public void GetRedPortion()
    {
        HP = Math.Min(MaxHP, HP + Random.Range(5, 10));
    }
}
