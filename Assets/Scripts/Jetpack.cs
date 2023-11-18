using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour
{
    [SerializeField] TrailRenderer jetpackTrail;
    [SerializeField] PlayerMovement playerMovement;

    //currently not needed, but might have the jetpack do some kind of overheating
    //animation/particle in the future.
    [SerializeField] FuelSystem fuelSystem;


    private void Update() {
        if(playerMovement.IsGrounded){
            jetpackTrail.emitting = false;
        } else {
            jetpackTrail.emitting = true;
        }

    }
}
