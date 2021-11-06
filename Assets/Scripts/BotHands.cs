using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotHands : MonoBehaviour
{
    public GameObject knife;
    public GameObject pistol;
    public GameObject mainGun;

    public void ApplyWeapon(int weapon) => eventsToRaise.Enqueue(() => _ApplyWeapon(weapon));

    private void _ApplyWeapon(int weapon)
    {
        switch (weapon)
        {
            case 0:
                knife.SetActive(true);
                pistol.SetActive(false);
                mainGun.SetActive(false);
                break;
            case 1:
                knife.SetActive(false);
                pistol.SetActive(true);
                mainGun.SetActive(false);
                break;
            case 2:
                knife.SetActive(false);
                pistol.SetActive(false);
                mainGun.SetActive(true);
                break;
        }
        Debug.LogWarning("change weapon applyed!");
    }

    private void FixedUpdate()
    {
        while (eventsToRaise.Any())
        {
            eventsToRaise.Dequeue().Invoke();
        }   
    }
    
    Queue<Action> eventsToRaise = new Queue<Action>();
}
