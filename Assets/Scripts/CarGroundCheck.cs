using UnityEngine;

public class CarGroundCheck : MonoBehaviour
{
    public WheelCollider[] wheels; // Array to hold all the WheelColliders
    public float unstuckForce = 5f; // Force applied to unstuck the car
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckGrounded();
    }

    void CheckGrounded()
    {
        bool isGrounded = false;

        // Check if any of the wheels are grounded
        foreach (WheelCollider wheel in wheels)
        {
            if (wheel.isGrounded)
            {
                isGrounded = true; // If at least one wheel is grounded, we stop checking
                break;
            }
        }

        // If no wheel is grounded, apply a downward force to unstuck the car
        if (!isGrounded)
        {
            // Apply a force downward to push the car back onto the ground
            rb.AddForce(Vector3.down * unstuckForce, ForceMode.Impulse);
        }
    }
}
