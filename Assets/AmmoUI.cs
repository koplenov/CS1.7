using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoUI;
    void Start()
    {
        ammoUI.text = "HP: ";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
