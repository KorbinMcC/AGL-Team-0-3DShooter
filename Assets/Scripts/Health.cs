using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 200f;

    public event Action OnHealthChanged;
    
    private float currentHealth;
    private bool isDead = false;
    public bool IsDead{ get{ return isDead; } }

    private void Awake() {
        currentHealth = maxHealth;
    }

    private void Start() {
        //Update Health Bar
        OnHealthChanged?.Invoke();
    }

    //TEMPORARY, FOR TESTING DAMAGE
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Q)){
            if(gameObject.tag == "Player"){
                TakeDamage(20);
            }
        }
        if(Input.GetKeyDown(KeyCode.E)){
            if(gameObject.tag == "Enemy"){
                TakeDamage(20);
            }
        }
    }

    public float GetHealthPercentage(){
        return currentHealth / maxHealth;
    }

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;

    public void TakeDamage(float damageAmount){
        if(isDead) { return; }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0f);

        print($"{name} took {damageAmount} damage, is now at {currentHealth} health.");

        if(currentHealth <= Mathf.Epsilon){
            isDead = true;
        }

        //Update Health Bar
        OnHealthChanged?.Invoke();
    }


    
}
