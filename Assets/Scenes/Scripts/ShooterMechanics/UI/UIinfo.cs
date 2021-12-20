using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIinfo : MonoBehaviour
{
    public Selfstate info;
    public TextMeshProUGUI hpview;
    public TextMeshProUGUI damageview;
    void Awake()
    {
        
    }

    
    void Update()
    {
        hpview.text = "HP: " + Client.Instance.hp;
        damageview.text = "$ " + Client.Instance.money;
    }
}
