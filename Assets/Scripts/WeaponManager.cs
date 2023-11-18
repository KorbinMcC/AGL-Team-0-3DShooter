using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public event Action OnWeaponChanged;

    [SerializeField] Transform aimTargetTransform;
    [SerializeField] Transform playerModelWeaponParent = null;

    [SerializeField] WeaponSO[] weapons;
    private int currentWeaponIndex = 0;

    private GameObject weaponModel = null;
    private WeaponSO currentWeaponSO = null;
    public WeaponSO CurrentWeapon{ get{ return currentWeaponSO; } }

    private bool hasAmmo = true;
    public bool HasAmmo{ get{ return hasAmmo; } }

    private float[] weaponReloadTimes;
    private int[] currentAmmo;

    FuelSystem fuelSystem;

    private void Awake() {
        fuelSystem = GetComponent<FuelSystem>();
        weaponReloadTimes = new float[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            weaponReloadTimes[i] = 0f;
        }
        currentAmmo = new int[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            currentAmmo[i] = weapons[i].magazineSize;
        }
        SetWeaponActive();
    }
    

    void Update()
    {
        CheckForWeaponChange();
        Vector3 mousePosition;

        mousePosition = FireRayAtCenterOfScreen();

        //can reload with R if your magazine is not full, or reload automatically when out of ammo.
        if( (Input.GetKeyDown(KeyCode.R) && currentAmmo[currentWeaponIndex] != currentWeaponSO.magazineSize)
            || currentAmmo[currentWeaponIndex] == 0){
            currentAmmo[currentWeaponIndex] = 0;
            hasAmmo = false;
            OnWeaponChanged?.Invoke();
        }

        if (hasAmmo)
        {
            ProcessWeaponFiring(mousePosition);
        }
        else
        {
            WeaponReloadingLogic();
        }

    }

    public int GetCurrentWeaponAmmo(){
        return currentAmmo[currentWeaponIndex];
    }

    public int GetCurrentWeaponMaximumAmmo(){
        return currentWeaponSO.magazineSize;
    }

    public float GetAmmoSliderPercentage(){
        if(hasAmmo){
            return (float)currentAmmo[currentWeaponIndex] / currentWeaponSO.magazineSize;
        } else {
            return weaponReloadTimes[currentWeaponIndex] / currentWeaponSO.reloadTime;
        }
    }

    private void CheckForWeaponChange()
    {
        int previousWeaponIndex = currentWeaponIndex;
        ProcessKeyInput();
        ProcessScrollWheelInput();
        if (previousWeaponIndex != currentWeaponIndex) { 
            SetWeaponActive();
        }
    }

    private void WeaponReloadingLogic()
    {
        if(currentWeaponSO.requiresFuelToReload){
            if(fuelSystem.HasFuel){
                weaponReloadTimes[currentWeaponIndex] += Time.deltaTime;
            }
        } else {
            weaponReloadTimes[currentWeaponIndex] += Time.deltaTime;
        }

        if(weaponReloadTimes[currentWeaponIndex] >= currentWeaponSO.reloadTime){
            weaponReloadTimes[currentWeaponIndex] = 0f;
            currentAmmo[currentWeaponIndex] = currentWeaponSO.magazineSize;
            hasAmmo = true;
            OnWeaponChanged?.Invoke();
        }
    }

    private void ProcessWeaponFiring(Vector3 mousePosition)
    {
        if(Input.GetMouseButtonDown(0)){
            currentAmmo[currentWeaponIndex] -= 1; 
            OnWeaponChanged?.Invoke();

            if(currentWeaponSO.projectile != null){
                GameObject proj = Instantiate(currentWeaponSO.projectile, playerModelWeaponParent.position, Quaternion.LookRotation(mousePosition));
                proj.GetComponent<Projectile>().SetDamage(currentWeaponSO.damagePerShot);
                proj.GetComponent<Projectile>().SetOwner(gameObject);

            } else {
                if(Physics.Raycast(playerModelWeaponParent.position, aimTargetTransform.position, out RaycastHit hit, currentWeaponSO.weaponRange)){
                    Debug.DrawLine(playerModelWeaponParent.position, aimTargetTransform.position, Color.red, 10f);
                    
                    if(hit.collider.gameObject.TryGetComponent<Health>(out Health health)){
                        health.TakeDamage(currentWeaponSO.damagePerShot);
                    }
                }
            }
        }

    }

    private Vector3 FireRayAtCenterOfScreen(){
        Vector3 mousePosition = Vector3.zero;

        Vector2 centerScreenPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(centerScreenPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 500f)){
            aimTargetTransform.position = raycastHit.point;
            mousePosition = raycastHit.point;
            print(raycastHit.point);
        }

        return mousePosition;
    }

    private void ProcessKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeaponIndex = 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeaponIndex = 1;
        }
    }


    private void ProcessScrollWheelInput()
    {
        //if scrolling up
        if(Input.GetAxis("Mouse ScrollWheel") > 0){
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        }
        //if scrolling down
        if(Input.GetAxis("Mouse ScrollWheel") < 0){
            currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
        }
    }

    private void SetWeaponActive()
    {
        //destroy the previous weapon model if any
        if(weaponModel != null){
            Destroy(weaponModel);
        }

        //set the current weapon to the one at the current index.
        currentWeaponSO = weapons[currentWeaponIndex];
        print($"Swapping to {currentWeaponSO.weaponName}");

        //then instantiate the weapon model of that new weapon
        if(currentWeaponSO.weaponPrefab != null){
            weaponModel = Instantiate(currentWeaponSO.weaponPrefab, playerModelWeaponParent);
        }

        if(currentAmmo[currentWeaponIndex] == 0){
            hasAmmo = false;
        } else {
            hasAmmo = true;
        }

        OnWeaponChanged?.Invoke();
    }

}
