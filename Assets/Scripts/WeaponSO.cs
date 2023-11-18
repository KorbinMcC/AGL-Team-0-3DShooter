using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "AGL-Team-0-3DShooter/WeaponSO", order = 0)]
public class WeaponSO : ScriptableObject 
{
    //the model of the weapon, 
    public GameObject weaponPrefab = null;
    public GameObject hitEffect = null;
    public GameObject projectile = null;

    public string weaponName = "Default Weapon";
    public int magazineSize = 30;
    public float damagePerShot = 1f;
    public float weaponRange = 100f;
    public float reloadTime = 3f;

    public bool requiresFuelToReload = false;
    public float fuelCostToReload = 30f;

}
