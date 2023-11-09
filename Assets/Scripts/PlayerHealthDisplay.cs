using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHealthDisplay : MonoBehaviour
{
    [Header("Health Bar Visual Settings")]
    [SerializeField] Slider healthBar = null;
    [SerializeField] Image healthBarFill = null;

    [SerializeField] TextMeshProUGUI currentHealthText = null;
    [SerializeField] TextMeshProUGUI maxHealthText = null;

    [SerializeField] [Range(0,1f)] float criticalHealthPercent = .25f;
    [SerializeField] Color naturalHealthColor;
    [SerializeField] Color criticalHealthColor;

    Health playerHealth;

    private void Awake() {
        playerHealth = GetComponent<Health>();
    }

    private void OnEnable() {
        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable() {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void Start() {
        if(maxHealthText != null){
            maxHealthText.text = String.Format("{0:0}", playerHealth.GetMaxHealth());
        }
    }

    private void UpdateHealthBar()
    {
        if(healthBar == null) { return; }

        if(currentHealthText != null){
            currentHealthText.text = String.Format("{0:0}", playerHealth.GetCurrentHealth());
        }

        if(playerHealth.IsDead){
            healthBarFill.enabled = false;
            return;
        }

        float healthPercent = playerHealth.GetHealthPercentage();

        if (healthPercent < criticalHealthPercent){
            healthBarFill.color = criticalHealthColor;
        } else {
            healthBarFill.color = naturalHealthColor;
        }

        healthBar.value = healthPercent;
    }
}
