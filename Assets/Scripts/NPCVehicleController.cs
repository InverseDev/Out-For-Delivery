using UnityEngine;

public class NPCVehicleMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 2f;
    public float deceleration = 3f;
    public float detectionDistance = 10f;
    public Vector3 boxSize = new Vector3(2f, 2f, 5f);
    public LayerMask obstacleLayer;
    public float hornCooldown = 2f;
    public float checkTime = 3f;
    public float minMoveDistance = 5f;
    public float turnSlowdownSpeed = 2f; 

    private float currentSpeed = 0f;
    private bool isPlayerInFront = false;
    private Rigidbody rb;
    private float lastHornTime = -Mathf.Infinity;
    private Vector3 lastPosition;
    private bool stopForPlayer = false;
    private float lastCheckTime;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool wasPlayerInFront = false;
    private NPCVehicleTurn turnController;
    private bool isRotationPaused = false;
    private float pausedRotationProgress = 0f;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        turnController = GetComponent<NPCVehicleTurn>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        lastPosition = transform.position;
        lastCheckTime = Time.time;
    }

    private void FixedUpdate()
    {
        CarMovement();
    }

    public void CarMovement()
    {
        isPlayerInFront = IsObstacleInFront();
        
        if (wasPlayerInFront && !isPlayerInFront)
        {
            AudioManager.Instance.StopHornSound();
        }

        float targetSpeed = moveSpeed;
        
        if (turnController != null && turnController.IsTurning)
        {
            targetSpeed = turnSlowdownSpeed;
        }

        if (isPlayerInFront)
        {
            targetSpeed = 0f;
            stopForPlayer = true;
            
            if (Time.time - lastHornTime > hornCooldown)
            {
                AudioManager.Instance.PlayHornSound(transform.position);
                lastHornTime = Time.time;
            }
        }
        else
        {
            stopForPlayer = false;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 
            (targetSpeed > currentSpeed ? acceleration : deceleration) * Time.fixedDeltaTime);

        wasPlayerInFront = isPlayerInFront;

        Vector3 movement = transform.forward * currentSpeed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        
        CheckForNoMovement();
    }

  private Vector3 CalculateDetectionBoxSize()
    {
        Collider vehicleCollider = GetComponent<Collider>();
        
        if (vehicleCollider == null)
        {
            vehicleCollider = GetComponentInChildren<Collider>();
        }
        
        if (vehicleCollider != null)
        {
            Bounds bounds = vehicleCollider.bounds;
            
            return new Vector3(
                bounds.size.x + 3f,   
                bounds.size.y,        
                bounds.size.z + 5f    
            );
        }
        
        Debug.LogWarning("No collider found on NPC vehicle. Using default detection box size.");
        return new Vector3(5f, 2f, 10f);
    }
    private bool IsObstacleInFront()
    {
        Vector3 boxSize = CalculateDetectionBoxSize();

        RaycastHit[] hits = Physics.BoxCastAll(
            transform.position, 
            boxSize * 0.5f, 
            transform.forward, 
            transform.rotation, 
            detectionDistance, 
            obstacleLayer
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("PlayerCar"))
            {
                if (turnController != null)
                {
                    turnController.PauseRotation();
                    isRotationPaused = true;
                }
                
                Debug.Log("Obstacle (Player Car) detected at distance: " + hit.distance);
                return true;
            }
        }

        if (isRotationPaused)
        {
            if (turnController != null)
            {
                turnController.ResumeRotation(pausedRotationProgress);
            }
            isRotationPaused = false;
        }

        return false;

    }
    private void CheckForNoMovement()
    {
        if (!stopForPlayer && !turnController.IsTurning && (Time.time - lastCheckTime >= checkTime))
        {
            float distanceMoved = Vector3.Distance(lastPosition, transform.position);
            
            if (distanceMoved < minMoveDistance)
            {
                ReinstantiateVehicle();
            }

            lastCheckTime = Time.time;
            lastPosition = transform.position;
        }
    }

    private void ReinstantiateVehicle()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        currentSpeed = 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 boxCenter = transform.position + transform.forward * detectionDistance * 0.5f;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
