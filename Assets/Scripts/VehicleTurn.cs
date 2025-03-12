using UnityEngine;
using System.Collections;

public class NPCVehicleTurn : MonoBehaviour
{
    public float turnSpeed = 90f;    
    public bool turnRight = true;      
    public float cooldownTime = 1f;    
    public float turnDuration = 1f;    
   
    private bool isTurning = false;
    public bool IsTurning => isTurning;
   
    private Quaternion targetRotation;
    private Quaternion startRotation;
    private NPCVehicleMovement moveCar;
    private float turnTimer = 0f;
    
    // New variables for pause and resume functionality
    private bool isPaused = false;
    private float pausedTurnProgress = 0f;
    private float totalRotationProgress = 0f;
   
    private void Start()
    {
        moveCar = GetComponent<NPCVehicleMovement>();
    }

    private void FixedUpdate()
    {
        if (isTurning && !isPaused)
        {
            turnTimer += Time.fixedDeltaTime;
            float turnProgress = turnTimer / turnDuration;
            totalRotationProgress = turnProgress;
           
            if (turnProgress >= 1f)
            {
                transform.rotation = targetRotation;
                SnapToNearestRightAngle();
                isTurning = false;
                turnTimer = 0f;
                totalRotationProgress = 0f;
                StartCoroutine(TurnCooldown());
            }
            else
            {
                float t = Mathf.SmoothStep(0f, 1f, turnProgress);
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            }
        }
    }

    public void PauseRotation()
    {
        if (isTurning)
        {
            isPaused = true;
            pausedTurnProgress = totalRotationProgress;
        }
    }

    public void ResumeRotation(float additionalProgress = 0f)
    {
        if (isPaused)
        {
            isPaused = false;
            // Ensure we don't exceed 1 (full rotation)
            totalRotationProgress = Mathf.Min(pausedTurnProgress + additionalProgress, 1f);
            turnTimer = totalRotationProgress * turnDuration;
        }
    }

    public void TriggerTurn()
    {
        if (!isTurning)
        {
            InitiateTurn();
        }
    }

    private void InitiateTurn()
    {
        isTurning = true;
        turnTimer = 0f;
        startRotation = transform.rotation;
       
        float turnAngle = turnRight ? 90f : -90f;
        targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + turnAngle, 0);
    }

    private IEnumerator TurnCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
    }

    private void SnapToNearestRightAngle()
    {
        Vector3 angles = transform.rotation.eulerAngles;
        angles.y = Mathf.Round(angles.y / 90f) * 90f;
        transform.rotation = Quaternion.Euler(angles);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Waypoints") && !isTurning)
        {
            TriggerTurn();
        }
    }
}