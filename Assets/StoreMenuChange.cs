using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreMenuChange : MonoBehaviour
{
    [SerializeField, Header("1st Circle")] private GameObject gunTypes;
    [SerializeField] private Button type1;
    [SerializeField] private Button type2;
    [SerializeField] private Button type3;
    [SerializeField] private Button type4;
    [SerializeField] private Button type5;
    [SerializeField] private Button type6;
    [SerializeField, Header("2st Circle")] private GameObject guns1;
    [SerializeField] private Button gun1;
    [SerializeField] private Button gun2;
    [SerializeField] private Button gun3;
    [SerializeField] private Button gun4;
    [SerializeField] private Button gun5;
    [SerializeField] private Button gun6;
    [SerializeField, Header("3st Circle")] private GameObject guns2;
    [SerializeField] private Button gun7;
    [SerializeField] private Button gun8;
    [SerializeField] private Button gun9;
    [SerializeField] private Button gun10;
    [SerializeField] private Button gun11;
    [SerializeField] private Button gun12;
    [SerializeField, Header("4st Circle")] private GameObject guns3;
    [SerializeField] private Button gun13;
    [SerializeField] private Button gun14;
    [SerializeField] private Button gun15;
    [SerializeField] private Button gun16;
    [SerializeField] private Button gun17;
    [SerializeField] private Button gun18;
    [SerializeField, Header("5st Circle")] private GameObject guns4;
    [SerializeField] private Button gun19;
    [SerializeField] private Button gun20;
    [SerializeField] private Button gun21;
    [SerializeField] private Button gun22;
    [SerializeField] private Button gun23;
    [SerializeField] private Button gun24;
    [SerializeField, Header("6st Circle")] private GameObject guns5;
    [SerializeField] private Button gun25;
    [SerializeField] private Button gun26;
    [SerializeField] private Button gun27;
    [SerializeField] private Button gun28;
    [SerializeField] private Button gun29;
    [SerializeField] private Button gun30;
    [SerializeField, Header("7st Circle")] private GameObject guns6;
    [SerializeField] private Button gun31;
    [SerializeField] private Button gun32;
    [SerializeField] private Button gun33;
    [SerializeField] private Button gun34;
    [SerializeField] private Button gun35;
    [SerializeField] private Button gun36;

    void Start()
    {
        type1.onClick.AddListener(delegate { OnTypeClick(1); });
        type2.onClick.AddListener(delegate { OnTypeClick(2); });
        type3.onClick.AddListener(delegate { OnTypeClick(3); });
        type4.onClick.AddListener(delegate { OnTypeClick(4); });
        type5.onClick.AddListener(delegate { OnTypeClick(5); });
        type6.onClick.AddListener(delegate { OnTypeClick(6); });
        gun1.onClick.AddListener(delegate { OnGunClick(1,1); });
        gun2.onClick.AddListener(delegate { OnGunClick(1,2); });
        gun3.onClick.AddListener(delegate { OnGunClick(1,3); });
        gun4.onClick.AddListener(delegate { OnGunClick(1,4); });
        gun5.onClick.AddListener(delegate { OnGunClick(1,5); });
        gun6.onClick.AddListener(delegate { OnGunClick(1,6); });
        gun7.onClick.AddListener(delegate { OnGunClick(2,1); });
        gun8.onClick.AddListener(delegate { OnGunClick(2,2); });
        gun9.onClick.AddListener(delegate { OnGunClick(2,3); });
        gun10.onClick.AddListener(delegate { OnGunClick(2,4); });
        gun11.onClick.AddListener(delegate { OnGunClick(2,5); });
        gun12.onClick.AddListener(delegate { OnGunClick(2,6); });
        gun13.onClick.AddListener(delegate { OnGunClick(3,1); });
        gun14.onClick.AddListener(delegate { OnGunClick(3,2); });
        gun15.onClick.AddListener(delegate { OnGunClick(3,3); });
        gun16.onClick.AddListener(delegate { OnGunClick(3,4); });
        gun17.onClick.AddListener(delegate { OnGunClick(3,5); });
        gun18.onClick.AddListener(delegate { OnGunClick(3,6); });
        gun19.onClick.AddListener(delegate { OnGunClick(4,1); });
        gun20.onClick.AddListener(delegate { OnGunClick(4,2); });
        gun21.onClick.AddListener(delegate { OnGunClick(4,3); });
        gun22.onClick.AddListener(delegate { OnGunClick(4,4); });
        gun23.onClick.AddListener(delegate { OnGunClick(4,5); });
        gun24.onClick.AddListener(delegate { OnGunClick(4,6); });
        gun25.onClick.AddListener(delegate { OnGunClick(5,1); });
        gun26.onClick.AddListener(delegate { OnGunClick(5,2); });
        gun27.onClick.AddListener(delegate { OnGunClick(5,3); });
        gun28.onClick.AddListener(delegate { OnGunClick(5,4); });
        gun29.onClick.AddListener(delegate { OnGunClick(5,5); });
        gun30.onClick.AddListener(delegate { OnGunClick(5,6); });
        gun31.onClick.AddListener(delegate { OnGunClick(6,1); });
        gun32.onClick.AddListener(delegate { OnGunClick(6,2); });
        gun33.onClick.AddListener(delegate { OnGunClick(6,3); });
        gun34.onClick.AddListener(delegate { OnGunClick(6,4); });
        gun35.onClick.AddListener(delegate { OnGunClick(6,5); });
        gun36.onClick.AddListener(delegate { OnGunClick(6,6); });
        
    }
    
    void OnTypeClick(int typeid)
    {
        switch (typeid)
        {
            case 1:
                gunTypes.SetActive(false);
                guns1.SetActive(true);
                break;
            case 2:
                gunTypes.SetActive(false);
                guns2.SetActive(true);
                break;
            case 3:
                gunTypes.SetActive(false);
                guns3.SetActive(true);
                break;
            case 4:
                gunTypes.SetActive(false);
                guns4.SetActive(true);
                break;
            case 5:
                gunTypes.SetActive(false);
                guns5.SetActive(true);
                break;
            case 6:
                gunTypes.SetActive(false);
                guns6.SetActive(true);
                break;
        }
    }

    void OnGunClick(int typeid, int gunid)
    {
        switch (typeid,gunid)
        {
         case   (1,1): 
             guns1.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (1,2): 
             guns1.SetActive(false);
             gunTypes.SetActive(true);
             break;
         case    (1,3): 
             guns1.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         case    (1,4): 
             guns1.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (1,5): 
             guns1.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (1,6): 
             guns1.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         
         
         case   (2,1): 
             guns2.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (2,2): 
             guns2.SetActive(false);
             gunTypes.SetActive(true);
             break;
         case    (2,3): 
             guns2.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         case    (2,4): 
             guns1.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (2,5): 
             guns2.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (2,6): 
             guns2.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         
         
         case   (3,1): 
             guns3.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (3,2): 
             guns3.SetActive(false);
             gunTypes.SetActive(true);
             break;
         case    (3,3): 
             guns3.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         case    (3,4): 
             guns3.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (3,5): 
             guns3.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (3,6): 
             guns3.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         
         
         case   (4,1): 
             guns4.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (4,2): 
             guns4.SetActive(false);
             gunTypes.SetActive(true);
             break;
         case    (4,3): 
             guns4.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         case    (4,4): 
             guns4.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (4,5): 
             guns4.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (4,6): 
             guns4.SetActive(false); 
             gunTypes.SetActive(true);
             break;


         case   (5,1): 
             guns5.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (5,2): 
             guns5.SetActive(false);
             gunTypes.SetActive(true);
             break;
         case    (5,3): 
             guns5.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         case    (5,4): 
             guns5.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (5,5): 
             guns5.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (5,6): 
             guns5.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         
         
         case   (6,1): 
             guns6.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (6,2): 
             guns6.SetActive(false);
             gunTypes.SetActive(true);
             break;
         case    (6,3): 
             guns6.SetActive(false); 
             gunTypes.SetActive(true);
             break;
         case    (6,4): 
             guns6.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (6,5): 
             guns6.SetActive(false);  
             gunTypes.SetActive(true);
             break;
         case    (6,6): 
             guns6.SetActive(false); 
             gunTypes.SetActive(true);
             break;
        }
    }
}