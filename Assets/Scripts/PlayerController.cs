using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private const float HORIZONTAL_INPUT_SCALE = 50f; // m/s^2
    private const float VERTICAL_INPUT_SCALE = 50f; // m/s^2
    private const float MAX_MOVE_SPEED = 5f; // m/s
    private const float DECELERATION_RATE = 100; // m/s^2

    private const float HOVER_POWER = 20f; // m/s^2 (should overcome gravity)
    private const float ROCKET_VELOCITY_BOOST = 10f; // m/s

    private const float ROCKET_FIRE_DELAY = 0.8f;
    private const float ROCKET_SPEED = 30f;

    public GameObject rocketPrefab;
    public GameObject rocketLauncher;

    private float lastRocketFireTime = 0f;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start () {
        this.rigidBody = transform.parent.GetComponent<Rigidbody>();


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");

        bool spaceHeld = Input.GetKey(KeyCode.Space);
        bool shiftPressed = Input.GetKeyDown(KeyCode.LeftShift);

        bool rocketFired = Input.GetMouseButton(0); // left click

        Vector3 velocity = rigidBody.velocity;
        velocity = HandleHorizontalMovement(velocity, hInput, vInput);
        velocity = HandleVerticalMovement(velocity, spaceHeld, shiftPressed);

        rigidBody.velocity = velocity;

        if (rocketFired && CanFireRocket()) {
           FireRocket();
        }
    }

    private Vector3 HandleHorizontalMovement(Vector3 velocity, float hInput, float vInput) {
        Vector3 velocityChange = this.transform.right * hInput * HORIZONTAL_INPUT_SCALE +
    this.transform.forward * vInput * VERTICAL_INPUT_SCALE;
        velocityChange *= Time.deltaTime;


        // if flatVelocity is greater than MAX_MOVE_SPEED, add velocityChange and normalize to oldMagnitude
        // if newVelocity is greater than MAX_MOVE_SPEED decelerate towards 0

        Vector3 newVelocity = Vector3.zero;
        Vector3 flatVelocity = velocity;
        flatVelocity.y = 0f;
        newVelocity = flatVelocity + velocityChange;
        // we can't use walking to accelerate faster than our MAX_MOVE_SPEED
        float flatMagnitude = newVelocity.magnitude;
        if (flatMagnitude > MAX_MOVE_SPEED) {
            newVelocity = (newVelocity + velocityChange).normalized * flatMagnitude;
        }

        if (flatMagnitude > MAX_MOVE_SPEED || velocityChange.magnitude < 0.001f) {

            Vector3 decelerationVector = newVelocity.normalized * DECELERATION_RATE * Time.deltaTime;

            // If we started going in the opposite direction, just stop instead
            if (decelerationVector.magnitude > newVelocity.magnitude) {
                newVelocity = Vector3.zero;
            }
            else {
                newVelocity = newVelocity - decelerationVector;
            }

            // if we were goin faster than MAX_MOVE_SPEED and slowed down to less than it, but are still moving, then clamp
            if (newVelocity.magnitude < MAX_MOVE_SPEED && velocityChange.magnitude > 0.001f) {
                newVelocity = newVelocity.normalized * MAX_MOVE_SPEED;
            }
        }

        return new Vector3(newVelocity.x, rigidBody.velocity.y, newVelocity.z);
    }

    private Vector3 HandleVerticalMovement(Vector3 velocity, bool spaceHeld, bool shiftPressed) {
        float verticalVelocity = 0f;
        if (!IsGrounded()) {
            verticalVelocity += Physics.gravity.y * Time.deltaTime; 
        }

        if (spaceHeld) {
            verticalVelocity += HOVER_POWER * Time.deltaTime;
        }

        if (shiftPressed) {
            verticalVelocity = Mathf.Max(ROCKET_VELOCITY_BOOST, verticalVelocity + ROCKET_VELOCITY_BOOST);
        }

        return velocity + verticalVelocity * transform.up;
    }

    private void FireRocket() {
        lastRocketFireTime = Time.time;
        GameObject rocket = Instantiate(rocketPrefab);
        rocket.transform.position = rocketLauncher.transform.position + rocketLauncher.transform.up * 2f;
        rocket.transform.rotation = Quaternion.LookRotation(Camera.main.transform.up);

        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb) {
            rb.velocity = Camera.main.transform.forward * ROCKET_SPEED;
        }
    }

    private bool CanFireRocket() {
        return Time.time - lastRocketFireTime > ROCKET_FIRE_DELAY;
    }

    private bool IsGrounded() {
        Debug.DrawRay(transform.position, -transform.up * 1.2f, Color.red);
        return Physics.Raycast(transform.position, -transform.up, 1.2f);
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("COLLIDING: " + Time.time);
    }
}
