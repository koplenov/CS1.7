using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Networking;
using UnityEngine;
using Utils;
using Network = Utils.Network;

public abstract class Player : MonoBehaviour
{
    public int hp { get; set; }
    public int armor;
    public int money;

    protected Queue<Action> eventsToRaise = new Queue<Action>();

    protected void Awake()
    {
        hp = 100;
        armor = 0;
        money = 0;
    }

    private void LateUpdate()
    {
        while (eventsToRaise.Any())
        {
            eventsToRaise.Dequeue().Invoke();
        }   
    }

    public void AddMoney()
    {
        money += 1;
    }
    public void ApplyDamage(int damage)
    {
        eventsToRaise.Enqueue(() => _ApplyDamage(damage));
    }

    private void _ApplyDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        if (hp <= 0)
        {
            Respawn login = new Respawn(Client.nick);
            // content bytes
            byte[] tempBytes = Data.ObjectToByteArray(login);
            // bytes to send or bytes from server
            var byteData = Packer.CombinePacket(ChanelID.Respawn, tempBytes);
            //Login to the server
            Network.SendUdpData(byteData);
            hp = 100;
        }
    }
}
