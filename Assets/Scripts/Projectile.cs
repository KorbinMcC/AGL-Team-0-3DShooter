using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float velocity = 10f;
    private float damageAmount = 5f;
    private float projRange = 30f;

    private GameObject shooter = null;
    private Vector3 startingPos;

    public void SetDamage(float damage){
        damageAmount = damage;
    }

    public void SetOwner(GameObject owner){
        shooter = owner;
    }

    public void SetRange(float range){
        projRange = range;
    }

    private void Start() {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(startingPos, transform.position) > projRange){
            Destroy(gameObject);
        }

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
