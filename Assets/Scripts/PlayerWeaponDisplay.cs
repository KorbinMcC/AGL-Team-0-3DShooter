using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponDisplay : MonoBehaviour
{
    [Header("Weapon Visual Settings")]
    [SerializeField] TextMeshProUGUI currentAmmoText = null;
    [SerializeField] Slider ammoBar = null;
    [SerializeField] Image ammoBarFill = null;
    
    [SerializeField] Color hasAmmoColor;
    [SerializeField] Color reloadingColor;
    
    [SerializeField] TextMeshProUGUI weaponNamePanel = null;

    WeaponManager weaponManager;

    private void Awake() {
        weaponManager = GetComponent<WeaponManager>();
    }

    private void OnEnable() {
        weaponManager.OnWeaponChanged += UpdateWeaponDisplay;
    }

    private void OnDisable() {
        weaponManager.OnWeaponChanged -= UpdateWeaponDisplay;
    }


    private void Update() {
        float ammoPercent = weaponManager.GetAmmoSliderPercentage();

        if(weaponManager.HasAmmo && ammoBarFill.color != hasAmmoColor){
            ammoBarFill.color = hasAmmoColor;
        } 

        if(!weaponManager.HasAmmo && ammoBarFill.color != reloadingColor){
            ammoBarFill.color = reloadingColor;
        }

        ammoBar.value = ammoPercent;
    }

    private void UpdateWeaponDisplay()
    {
        if(weaponNamePanel != null){
            weaponNamePanel.text = weaponManager.CurrentWeapon.weaponName;
        }

        if(currentAmmoText != null){
            currentAmmoText.text = String.Format("{0} / {1}",
             weaponManager.GetCurrentWeaponAmmo(), weaponManager.GetCurrentWeaponMaximumAmmo());
        }
    }

}
