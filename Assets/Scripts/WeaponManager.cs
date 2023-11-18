using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public event Action OnWeaponChanged;

    //[SerializeField] Transform aimTargetTransform;
    [SerializeField] Transform playerModelWeaponParent = null;
    [SerializeField] Transform playerProjectileSpawnPoint = null;

    [SerializeField] GameObject weaponLine = null;

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
        
        //Was for trying to get player rig to look at target, not doing anything currently.
        // FireRayAtCenterOfScreen();

        //can reload with R if your magazine is not full, or reload automatically when out of ammo.
        if( (Input.GetKeyDown(KeyCode.R) && currentAmmo[currentWeaponIndex] != currentWeaponSO.magazineSize)
            || currentAmmo[currentWeaponIndex] == 0){
            currentAmmo[currentWeaponIndex] = 0;
            hasAmmo = false;
            OnWeaponChanged?.Invoke();
        }

        if (hasAmmo)
        {
            ProcessWeaponFiring();
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

    private void ProcessWeaponFiring()
    {
        //start by getting a raycast straight out of the camera.
        Physics.Raycast(Camera.main.transform.position,
                 Camera.main.transform.forward,
                 out RaycastHit hit,
                 100000f);
        //doing a raycast from player to center of screen has some issues, so
        //we do this to get "hit.point" so now we can go from player to hit.point which is nicer.
        //it's given a high range so it should almost always hit something.
        //if it doesn't just rely on the inaccurate camera transform forward.

        //if we didn't hit something with the initial raycast, the hit.point will be 0,0,0.
        //so if it didn't something, use the inaccurate camera aim, otherwise use hit.point.
        Vector3 fireDirection;
        if(hit.point == Vector3.zero){
            fireDirection = Camera.main.transform.forward;
        } else {
            fireDirection = (hit.point - playerProjectileSpawnPoint.position).normalized;
        }
        //there are invisible walls around the current playspace, so this *should* be unnecesssary.

        if(Input.GetMouseButtonDown(0)){
            currentAmmo[currentWeaponIndex] = Mathf.Max(currentAmmo[currentWeaponIndex] - 1, 0); 
            OnWeaponChanged?.Invoke();

            //if the weapon has an associated projectile, fire that
            if(currentWeaponSO.projectile != null){
                GameObject proj = Instantiate(currentWeaponSO.projectile,
                 playerProjectileSpawnPoint.position,
                 Quaternion.identity);
                
                //set up the projectile, giving it the direction to fly.
                proj.transform.forward = fireDirection;
                
                proj.GetComponent<Projectile>().SetDamage(currentWeaponSO.damagePerShot);
                proj.GetComponent<Projectile>().SetRange(currentWeaponSO.weaponRange);
                proj.GetComponent<Projectile>().SetOwner(gameObject);

            //otherwise just fire a raycast and deal damage if it hits
            } else {
                //if the raycast from the player hits something:
                if(Physics.Raycast(playerProjectileSpawnPoint.position,
                    fireDirection,
                    out RaycastHit newHit,
                    currentWeaponSO.weaponRange)){

                    //for testing, in scene view you can see a line where the aim hit.
                    Debug.DrawLine(playerProjectileSpawnPoint.position, newHit.point, Color.red, 10f);

                    //draw a line between the shooter and the target here
                    //use the player's projectile spawn point as the start, and newHit.point as the end.
                    GameObject line = Instantiate(weaponLine);
                    line.GetComponent<LineRenderer>().SetPosition(0, playerProjectileSpawnPoint.position);
                    line.GetComponent<LineRenderer>().SetPosition(1, newHit.point);
                    Destroy(line, 0.2f);

                    //if the weapon has a "hit effect", like some particles when it hits something, spawn it here.
                    //and spawn it using hit.normal, so the "up" of the effect faces where the cast came from.
                    if(currentWeaponSO.hitEffect != null){
                        Instantiate(currentWeaponSO.hitEffect, newHit.point, Quaternion.LookRotation(hit.normal));
                    }

                    //if the target has a health component, deal damage
                    if(newHit.collider.gameObject.TryGetComponent<Health>(out Health health)){
                        health.TakeDamage(currentWeaponSO.damagePerShot);
                    }
                //otherwise, just fire off a line that without logic.
                } else {
                    //draw a line between the shooter and the point at to their weapon's max range.
                    Vector3 verticalOffset = Vector3.up * 0.15f;

                    GameObject line = Instantiate(weaponLine);
                    line.GetComponent<LineRenderer>().SetPosition(0, playerProjectileSpawnPoint.position);
                    line.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                    Destroy(line, 0.2f);
                }
            }
        }
    }

    // private void FireRayAtCenterOfScreen(){
    //     Vector2 centerScreenPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    //     Ray ray = Camera.main.ScreenPointToRay(centerScreenPoint);
    //     if(Physics.Raycast(ray, out RaycastHit raycastHit, 500f)){
    //         aimTargetTransform.position = raycastHit.point;
    //     }
    // }

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
