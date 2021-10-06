using UnityEngine;

public class Weapon : MonoBehaviour
{
    public new string name;
    public int ammo;
    public int ammoInMagazine;
    public int currentAmmo;

    public virtual void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
        }
        else
        {
            Reload();
        }
    }

    public void Reload()
    {
        ammo -= (ammoInMagazine - currentAmmo);
        currentAmmo = ammoInMagazine;
        Debug.Log("Reloaded");
    }
}
