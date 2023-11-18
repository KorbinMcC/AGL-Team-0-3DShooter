using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float groundedMoveSpeed = 5f;
    [SerializeField] private float flyingMoveSpeed = 8f;
    [SerializeField] private float jumpForce = 225f;
    [SerializeField] private float jetpackThrustForce = 16f;

    [Header("Vertical Speed Caps")]
    [SerializeField] private float upwardSpeedCap = 20f;
    [SerializeField] private float downwardSpeedCap = -40f;

    Rigidbody playerRigidbody;
    FuelSystem fuelSystem;
    Health playerHealth;
    [SerializeField] Animator animator;

    //strings used for animations:
    private readonly int GroundedSpeedX = Animator.StringToHash("GroundedSpeedX");
    private readonly int GroundedSpeedZ = Animator.StringToHash("GroundedSpeedZ");
    private readonly int OnJump = Animator.StringToHash("OnJump");
    private readonly int CancelFlight = Animator.StringToHash("CancelFlight");
    private readonly int IsNotGrounded = Animator.StringToHash("IsNotGrounded");



    private bool isGrounded = false;
    public bool IsGrounded{ get{ return isGrounded; } }

    private bool isFlying = false;
    //property, basically works like a getter function
    public bool IsFlying{ get{ return isFlying; } }

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        fuelSystem = GetComponent<FuelSystem>();
        playerHealth = GetComponent<Health>();
    }

    void Update()
    {
        //check if the player is dead, if so don't allow movement
        if(playerHealth.IsDead){ 
            isFlying = false;
            playerRigidbody.velocity = new Vector3(0f, playerRigidbody.velocity.y, 0f);
            return; 
        }

        //two different speeds, one for grounded, one for while in the air
        float moveSpeed = isGrounded ? groundedMoveSpeed : flyingMoveSpeed;

        playerRigidbody.velocity = CalculateMovement(moveSpeed);

        //for flying (the || check means if you are holding jump while falling and hit the ground,
        //you will start flying again without needing to jump)
        //also check if the player currently has fuel.
        if(Input.GetButton("Jump") && (!isGrounded || isFlying) && fuelSystem.HasFuel){
            animator.ResetTrigger(CancelFlight);
            animator.ResetTrigger(OnJump);
            isFlying = true;
        } else {
            isFlying = false;
        }

        //for just jumping
        if(Input.GetButtonDown("Jump") && isGrounded){
            playerRigidbody.AddForce(Vector3.up * jumpForce);
            animator.SetTrigger(OnJump);
        }

        if(!isFlying && isGrounded){
            animator.SetTrigger(CancelFlight);
        }

        if(isGrounded){
            animator.SetBool(IsNotGrounded, false);
        } else {
            animator.SetBool(IsNotGrounded, true);
            animator.ResetTrigger(CancelFlight);
        }

        //change the player's walking animation
        PlayerAnimation();

        //prevent vertical speed from getting too high or low.
        ClampVerticalSpeed();
    }

    private void FixedUpdate() {
        if(isFlying){
            playerRigidbody.AddForce(Vector3.up * jetpackThrustForce);
        }

    }

    //will later have to add checks that we actually land on something ground based
    private void OnTriggerEnter(Collider other) {
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other) {
        isGrounded = false;
    }


    void PlayerAnimation() {
		//Calculate Input Vectors
		float InputX = Input.GetAxis("Horizontal");
		float InputZ = Input.GetAxis("Vertical");

		//Calculate the Input Magnitude

		animator.SetFloat(GroundedSpeedX, InputX, .1f, Time.deltaTime);
	    animator.SetFloat(GroundedSpeedZ, InputZ, .1f, Time.deltaTime);
		
	}

    private Vector3 CalculateMovement(float currentMoveSpeed){
        //store the forward and right direction in their own vectors
        Vector3 forwardMovement = Camera.main.transform.forward;
        Vector3 horizontalMovement = Camera.main.transform.right;

        //set the y to 0 so we don't care about any tilt to the camera
        forwardMovement.y = 0f;
        horizontalMovement.y = 0f;

        //normalize to make their values 1.
        forwardMovement.Normalize();
        horizontalMovement.Normalize();

        Vector3 combinedForwardMovement = currentMoveSpeed * Input.GetAxis("Vertical") * forwardMovement;
        Vector3 verticalVelocity = new Vector3(0, playerRigidbody.velocity.y, 0);
        Vector3 combinedHorizontalMovement = currentMoveSpeed * Input.GetAxis("Horizontal") * horizontalMovement;

        return combinedForwardMovement + verticalVelocity + combinedHorizontalMovement;
    }

    private void ClampVerticalSpeed()
    {
        Vector3 clampedVerticalVelocity = playerRigidbody.velocity;
        //clamp our vertical velocity between the downward and upward cap.
        clampedVerticalVelocity.y = Mathf.Clamp(playerRigidbody.velocity.y, downwardSpeedCap, upwardSpeedCap);
        playerRigidbody.velocity = clampedVerticalVelocity;
    }

}
