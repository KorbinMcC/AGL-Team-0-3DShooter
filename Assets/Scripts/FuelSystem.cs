using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] private float maxFuelAmount = 100f;

    [Header("Fuel Drain and Regen Rates")]
    [SerializeField] private float flyingFuelDrainRate = 10f;
    [SerializeField] private float naturalFuelRegenRate = 8f;
    [SerializeField] private float overheatedFuelRegenRate = 16f;
    
   
    [Header("Fuel Meter Settings")]
    [SerializeField] Slider fuelMeter = null;
    [SerializeField] Image fuelMeterFill = null;
    [SerializeField] [Range(0,1f)] float warningFuelPercent = .20f;
    [SerializeField] Color naturalFuelColor;
    [SerializeField] Color warningFuelColor;
    [SerializeField] Color overheatingFuelColor;

    
    PlayerMovement playerMovement;
    WeaponManager weaponManager;
    private float currentFuelAmount;

    private bool hasFuel = true;
    public bool HasFuel{ get{ return hasFuel; } }

    private void Awake() {
        currentFuelAmount = maxFuelAmount;
        playerMovement = GetComponent<PlayerMovement>();
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        if(hasFuel){
            //actions when player has fuel
            CalculateFuelFromFlying();
            DrainFuelWhileReloading();
        } else {
            //overheating, player can't do fuel based actions until it fully refills.
            currentFuelAmount = Mathf.Min(currentFuelAmount + (overheatedFuelRegenRate * Time.deltaTime), maxFuelAmount);
            if(currentFuelAmount == maxFuelAmount){
                hasFuel = true;
            }
        }

        //floats can have weird rounding errors trying to directly compare to 0
        //so compare with Mathf.Epsilon, which is ever so slightly higher than 0.
        if(currentFuelAmount <= Mathf.Epsilon){
            //player has run out of fuel
            hasFuel = false;
        }

        //update the visual fuel meter
        UpdateFuelMeter();
    }

    public float GetFuelPercentage(){
        return currentFuelAmount / maxFuelAmount;
    }

    private void UpdateFuelMeter()
    {
        if(fuelMeter == null) { return; }

        float fuelPercent = GetFuelPercentage();

        if(!hasFuel){
            fuelMeterFill.color = overheatingFuelColor;
        } else if (fuelPercent < warningFuelPercent){
            fuelMeterFill.color = warningFuelColor;
        } else {
            fuelMeterFill.color = naturalFuelColor;
        }

        fuelMeter.value = GetFuelPercentage();
    }

    private void CalculateFuelFromFlying()
    {
        //if the player is currently flying
        if (playerMovement.IsFlying)
        {
            //drain fuel at our flying fuel drain rate per second
            //but have it stop at 0 instead of going into negatives.
            currentFuelAmount = Mathf.Max(currentFuelAmount - (flyingFuelDrainRate * Time.deltaTime), 0f);
        }
        else
        {
            //otherwise regen fuel at our natural fuel rate per second
            //but cap it at our max fuel amount.
            currentFuelAmount = Mathf.Min(currentFuelAmount + (naturalFuelRegenRate * Time.deltaTime), maxFuelAmount);
        }
    }

    private void DrainFuelWhileReloading()
    {
        //if the player is currently reloading
        if (!weaponManager.HasAmmo && weaponManager.CurrentWeapon.requiresFuelToReload)
        {
            //drain fuel at that weapon's fuel cost to reload per second
            //but have it stop at 0 instead of going into negatives.
            currentFuelAmount = Mathf.Max(currentFuelAmount - (weaponManager.CurrentWeapon.fuelCostToReload * Time.deltaTime), 0f);
        }
        //don't increase fuel otherwise, since that would stack with flying fuel gain.
    }
}
