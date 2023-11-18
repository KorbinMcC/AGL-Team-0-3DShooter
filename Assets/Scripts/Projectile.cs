using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float velocity = 10f;
    private float damageAmount = 5f; 
    private GameObject shooter = null;

    public void SetDamage(float damage){
        damageAmount = damage;
    }

    public void SetOwner(GameObject owner){
        shooter = owner;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * velocity * transform.forward;
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject == shooter) { return; }

        print("Touched something.");
        if(other.gameObject.TryGetComponent<Health>(out Health health)){
            health.TakeDamage(damageAmount);
        }
        Destroy(gameObject);
    }
}
