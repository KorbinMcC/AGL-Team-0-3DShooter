using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    Health enemyHealth;

    private void Awake() {
        enemyHealth = GetComponent<Health>();
    }

    private void OnEnable() {
        enemyHealth.OnHealthChanged += TookDamage;
    }

    private void OnDisable() {
        enemyHealth.OnHealthChanged -= TookDamage;
    }

    private void TookDamage(){
        if(enemyHealth.IsDead){
            Destroy(gameObject);
        }
    }


}
