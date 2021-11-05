using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;
using Utils;

public class BotState : MonoBehaviour
{
    public int hp;
    public int armor;

    void Awake()
    {
        hp = 100;
        armor = 0;
    }


    void Update()
    {
    }

    public void ApplyDamage()
    {
        hp -= 1;
        if (hp <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        gameObject.transform.position = new Vector3(0, 4, -2);
        hp = 100;
    }
}