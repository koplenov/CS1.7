using System;
using System.Collections.Generic;
using System.Linq;
using Networking;
using UnityEngine;
using Utils;
using Network = Utils.Network;

public class Hands : MonoBehaviour
{
    public Transform gunPoint;

    public Weapon knife;
    public Weapon pistol;
    public Weapon mainGun;
    public Selfstate selfState;
    private Weapon curentWeapon;
    
    public GameObject decalGameObject;
    private void Start()
    {
        curentWeapon = knife;
        
        damage = 5;
    }

    public int damage;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            curentWeapon = knife;

            damage = 5;
            
            knife.gameObject.SetActive(true);
            pistol.gameObject.SetActive(false);
            mainGun.gameObject.SetActive(false);

            ApplyWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            curentWeapon = pistol;
            
            damage = 2;
            
            knife.gameObject.SetActive(false);
            pistol.gameObject.SetActive(true);
            mainGun.gameObject.SetActive(false);

            ApplyWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            curentWeapon = mainGun;
            
            damage = 4;
            
            knife.gameObject.SetActive(false);
            pistol.gameObject.SetActive(false);
            mainGun.gameObject.SetActive(true);

            ApplyWeapon(2);
        }

        // shoot    
        if (Input.GetMouseButtonDown(0))
        {
            curentWeapon.Shoot();

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                // decal
                SpawnDecal(hitInfo.point, hitInfo.normal);
                ApplySpawnDecal(hitInfo.point, hitInfo.normal);

                // damage
                if (hitInfo.collider.CompareTag("NetPlayer"))
                {
                    NetPlayerData netData = hitInfo.collider.GetComponent<NetPlayerData>();
                    GameUtils.SendDamage(Client.nick,netData.nick,damage);
                    netData.botState.ApplyDamage(damage);
                    Debug.Log("Fired");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            curentWeapon.Reload();
        }
        
        while (eventsToRaise.Any())
        {
            eventsToRaise.Dequeue().Invoke();
        }   
    }
    Queue<Action> eventsToRaise = new Queue<Action>();
    
    public void SpawnDecal(SpawnDecal spawnDecal)
    {
        SpawnDecal(spawnDecal.HitInfoPoint, spawnDecal.HitInfoNormal);
    }
    public void SpawnDecal(Vector3 hitInfoPoint, Vector3 hitInfoNormal)
    {
        var rot = Quaternion.FromToRotation(Vector3.up, hitInfoNormal);
        //var decal = Instantiate(decalGameObject, hitInfoPoint, rot);
        eventsToRaise.Enqueue(() => Instantiate(decalGameObject, hitInfoPoint, rot));
        Debug.Log("заскамил декаль");
    }
    private void ApplySpawnDecal(Vector3 hitInfoPoint, Vector3 hitInfoNormal)
    {
        // content bytes
        byte[] data = Data.ObjectToByteArray( new SpawnDecal(Client.nick, hitInfoPoint, hitInfoNormal));
        data = Packer.CombinePacket(ChanelID.SpawnDecal, data);
        Network.SendUdpData(data);
    }
    public static void ApplyWeapon(int weapon)
    {
        // content bytes
        byte[] data = Data.ObjectToByteArray(new ChangeWeapon(Client.nick, weapon));
        data = Packer.CombinePacket(ChanelID.ChangeWeapon, data);
        Network.SendUdpData(data);
    }
    
    #region GUI

    void OnGUI()
    {
        float
            Size_Whith = 10, //ширина блока
            Size_Heigh = 10, //высота Centr_Box_Heigh
            //центр блока
            Centr_Box_Whith = Size_Whith / 2, //центр блока по ширине
            Centr_Box_Heigh = Size_Heigh / 2, //центр блока по высоте
            //получим центр окна
            Screen_Whith_centr = Screen.width / 2,
            Screen_Heigh_centr = Screen.height / 2,
            //получаем точку начала рисования блока от центра экрана  отнимаем центр блока
            Position_box_Whith = Screen_Whith_centr - Centr_Box_Whith,
            Position_box_Heigh = Screen_Heigh_centr - Centr_Box_Heigh;

        /*позиция блока*/
        Vector2 Box_position = new Vector2(Position_box_Whith, Position_box_Heigh);
        /*ширина блока*/
        Vector2 Box_size = new Vector2(Size_Whith, Size_Heigh);
        /*задаем прямоугольную область*/
        Rect Box_Rect = new Rect(Box_position, Box_size);
        //рисуем блок в центре экрана
        GUI.Box(Box_Rect, "+");
        
        DrawAmmo();
    }

    void DrawAmmo()
    {
        // ammo
        GUI.Box(new Rect(Screen.width-100, Screen.height-40,100,40), $"{curentWeapon.name}\n{curentWeapon.currentAmmo}/{curentWeapon.ammo}");
        
        // client info
        GUI.Label(new Rect(0, 0, 100, 40), $"nick: {Client.nick}");
    }

    #endregion
}