using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selfstate : MonoBehaviour
{
    
    public int hp;
    public int armor;
    public int damage;
    void Awake()
    {
        hp = 100;
        armor = 0;
        damage = 0;
    }
    
    
    void Update()
    {
        
    }

    public void ApplyDamage()
    {
        damage += 1;
    }
}

