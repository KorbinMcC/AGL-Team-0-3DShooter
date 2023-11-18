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

    // private void Start() {
    //     if(maxHealthText != null){
    //         maxHealthText.text = String.Format("{0:0}", playerHealth.GetMaxHealth());
    //     }
    // }
    private void Update() {
        float ammoPercent = weaponManager.GetAmmoSliderPercentage();

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
