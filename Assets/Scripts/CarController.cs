using UnityEngine;
using CandyCoded.HapticFeedback;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float minSteerAngle = 10f;
    [SerializeField] private float steeringSmoothing = 5f;
    [SerializeField] private float accelerationSmoothing = 2f;
    [SerializeField] private float decelerationRate = 3f;
    [SerializeField] private float collisionThreshold = 3f;
    [SerializeField] private float maxSpeed = 150f;
    [SerializeField] private AudioSource CollisionSound;

    [Header("Steering Speed Dependency")]
    [SerializeField] private float maxSteerSpeed = 100f; 

    [Header("Anti-Drift Settings")]
    [SerializeField] private float sidewaysFrictionStiffness = 2f;
    [SerializeField] private float forwardFrictionStiffness = 1.5f;
    [SerializeField] private float antiRollForce = 5000f;

    [Header("Steering Wheel Control")]
    [SerializeField] private RectTransform steeringWheelButton;
    [SerializeField] private float maxSteeringWheelRotation = 90f;
    private float currentSteeringWheelRotation = 0f;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private bool isReversing;
    private bool isBraking;
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentMotorForce;
    private bool isAccelerating;
    private Rigidbody rb;
    private float currentSpeed;
    private bool isReverseBreaking;

    public float MaxSpeed => maxSpeed;

     [Header("Control Settings")]
    [SerializeField] private bool useTiltControls = false;
    [SerializeField] private float tiltSensitivity = 5f;
    [SerializeField] private Button tiltButton;
    [SerializeField] private Button joystickButton;
    [SerializeField] private Button wheel;
    [SerializeField] private Button camButton;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera fpsCamera;
    public bool isFPSView = false;
     private bool hasItem = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        ConfigureWheelColliders();

        tiltButton.onClick.AddListener(EnableTiltControls);
        joystickButton.onClick.AddListener(EnableJoystickControls);
        camButton.onClick.AddListener(SwitchCamera);

        EnableJoystickControls();

        mainCamera.gameObject.SetActive(true);
        fpsCamera.gameObject.SetActive(false);

        EventTrigger trigger = steeringWheelButton.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = steeringWheelButton.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { OnSteeringWheelDrag((PointerEventData)data); });
        trigger.triggers.Add(dragEntry);

        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener((data) => { ResetSteeringWheel(); });
        trigger.triggers.Add(endDragEntry);
    }

    private void Update()
    {
        if (useTiltControls)
        {
            float tilt = Input.acceleration.x * tiltSensitivity;
            horizontalInput = Mathf.Clamp(tilt, -1f, 1f);
        }
    }


    private void FixedUpdate()
    {
        UpdateCurrentSpeed();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        ApplyAntiRollForce();
        LimitSpeed();

        if (isBraking && currentSpeed < 1f)
        {
            isBraking = false;
            if (isReversing)
            {
                verticalInput = -1f;
            }
        }

        if (isReverseBreaking && currentSpeed < 1f)
        {
            isReverseBreaking = false;
            verticalInput = 1f;
        }
    }


    public void PickUpItem()
    {
        if (!hasItem)
        {
            hasItem = true;
            Debug.Log("Item picked up!");
         }
    }

    public void DeliverItem()
    {
        if (hasItem)
        {
            hasItem = false;
            Debug.Log("Item delivered!");
        }
    }

    private void UpdateCurrentSpeed()
    {
        currentSpeed = rb.linearVelocity.magnitude * 3.6f;
    }
    private void ConfigureWheelColliders()
        {
            WheelCollider[] wheelColliders = { frontLeftWheelCollider, frontRightWheelCollider, rearLeftWheelCollider, rearRightWheelCollider };
            foreach (WheelCollider wheel in wheelColliders)
            {
                WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
                sidewaysFriction.stiffness = sidewaysFrictionStiffness;
                wheel.sidewaysFriction = sidewaysFriction;

                WheelFrictionCurve forwardFriction = wheel.forwardFriction;
                forwardFriction.stiffness = forwardFrictionStiffness;
                wheel.forwardFriction = forwardFriction;
            }
        }
     public void EnableTiltControls()
    {
        if (!useTiltControls)
        {
            useTiltControls = true;
            UpdateControlButtonStates();
            ResetSteeringInput();
            HapticFeedback.LightFeedback();
        }
    }

    private float steeringWheelSmoothVelocity;
    private bool isVibrating = false;
    public void OnSteeringWheelDrag(PointerEventData eventData)
    {
        if (!useTiltControls)
        {
            if (!isVibrating)
            {    
                HapticFeedback.LightFeedback();
                Debug.Log("Vibrating!!");
                isVibrating = true;
            }
            }
            float targetRotation = currentSteeringWheelRotation - eventData.delta.x;
            targetRotation = Mathf.Clamp(targetRotation, -maxSteeringWheelRotation, maxSteeringWheelRotation);

            currentSteeringWheelRotation = Mathf.SmoothDamp(
                currentSteeringWheelRotation, 
                targetRotation, 
                ref steeringWheelSmoothVelocity, 
                0.01f
            );

            steeringWheelButton.rotation = Quaternion.Euler(0, 0, currentSteeringWheelRotation);

            horizontalInput = -currentSteeringWheelRotation / maxSteeringWheelRotation;

    }


    private Coroutine resetSteeringCoroutine;
    private void ResetSteeringWheel()
    {
        if (resetSteeringCoroutine != null)
        {
            StopCoroutine(resetSteeringCoroutine);
        }
        isVibrating = false;
        resetSteeringCoroutine = StartCoroutine(SmoothResetSteeringWheel());
    }
    private IEnumerator SmoothResetSteeringWheel()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        float startRotation = currentSteeringWheelRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            currentSteeringWheelRotation = Mathf.Lerp(startRotation, 0f, t);
            steeringWheelButton.rotation = Quaternion.Euler(0, 0, currentSteeringWheelRotation);

            horizontalInput = -currentSteeringWheelRotation / maxSteeringWheelRotation;

            yield return null;
        }

        currentSteeringWheelRotation = 0f;
        horizontalInput = 0f;
        steeringWheelButton.rotation = Quaternion.identity;
        HapticFeedback.LightFeedback();
    }

    public void EnableJoystickControls()
    {
        if (useTiltControls)
        {
            useTiltControls = false;
            UpdateControlButtonStates();
            ResetSteeringInput();
            ResetSteeringWheel();
            HapticFeedback.LightFeedback();
        }
    }

    private void UpdateControlButtonStates()
    {
        tiltButton.interactable = !useTiltControls;
        joystickButton.interactable = useTiltControls;
        wheel.interactable = !useTiltControls;
    }

    private void ResetSteeringInput()
    {
        horizontalInput = 0f;
    }

    public void SwitchCamera()
    {
        isFPSView = !isFPSView;
        
        if (isFPSView)
        {
            mainCamera.gameObject.SetActive(false);
            fpsCamera.gameObject.SetActive(true);
        }
        else
        {
            mainCamera.gameObject.SetActive(true);
            fpsCamera.gameObject.SetActive(false);
        }

        HapticFeedback.LightFeedback();
    }

    public void OnPressLeft()
    {
        if (!useTiltControls)
        {
            horizontalInput = -1f;
            HapticFeedback.LightFeedback();
        }
    }

    public void OnReleaseLeft()
    {
        if (!useTiltControls)
        {
            horizontalInput = 0f;
            HapticFeedback.LightFeedback();
        }
    }

    public void OnPressRight()
    {
        if (!useTiltControls)
        {
            horizontalInput = 1f;
            HapticFeedback.LightFeedback();
        }
    }

    public void OnReleaseRight()
    {
        if (!useTiltControls)
        {
            horizontalInput = 0f;
            HapticFeedback.LightFeedback();
        }
    }

    public void OnPressAccelerate()
    {
        HapticFeedback.LightFeedback();

        if (currentSpeed < -1f) 
        {
            isReverseBreaking = true;
            verticalInput = 0f;
        }
        else
        {
            isAccelerating = true;
            verticalInput = 1f;
        }
    }

    public void OnReleaseAccelerate()
    {
        isAccelerating = false;
        isReverseBreaking = false;
        verticalInput = 0f; 
        HapticFeedback.LightFeedback();
    }

    public void OnPressReverse()
    {
        HapticFeedback.LightFeedback();
        isReversing = true;
        if (currentSpeed > 1f) 
        {
            isBraking = true;
            verticalInput = 0f;
        }
        else
        {
            isBraking = false;
            verticalInput = -1f; 
        }
    }

    public void OnReleaseReverse()
    {
        HapticFeedback.LightFeedback();
        isReversing = false;
        isBraking = false;
        verticalInput = 0f;
    }
    private void HandleMotor()
    {
        if (isBraking || isReverseBreaking)
        {
            ApplyBraking(brakeForce * 2f);
            currentMotorForce = 0f;
        }
        else
        {
            float targetMotorForce = verticalInput * motorForce;
            currentMotorForce = Mathf.Lerp(currentMotorForce, targetMotorForce, Time.fixedDeltaTime * (isAccelerating || isReversing ? accelerationSmoothing : decelerationRate));

            frontLeftWheelCollider.motorTorque = currentMotorForce;
            frontRightWheelCollider.motorTorque = currentMotorForce;

            if (!isAccelerating && !isReversing)
            {
                ApplyBraking(brakeForce * 0.1f);
            }
            else
            {
                ApplyBraking(0f);
            }
        }
    }

    private void ApplyBraking(float brakeForce)
    {
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
    }

    private void HandleSteering()
    {
        float speedFactor = Mathf.Clamp01(currentSpeed / maxSteerSpeed);
        float currentMaxSteerAngle = Mathf.Lerp(maxSteerAngle, minSteerAngle, speedFactor);

        float targetSteerAngle = currentMaxSteerAngle * horizontalInput;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.fixedDeltaTime * steeringSmoothing);
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.SetPositionAndRotation(pos, rot);
    }

    private void ApplyAntiRollForce()
    {
        ApplyAntiRollForceToWheelPair(frontLeftWheelCollider, frontRightWheelCollider);
        ApplyAntiRollForceToWheelPair(rearLeftWheelCollider, rearRightWheelCollider);
    }

    private void ApplyAntiRollForceToWheelPair(WheelCollider wheelL, WheelCollider wheelR)
    {
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = wheelL.GetGroundHit(out WheelHit hitL);
        if (groundedL)
            travelL = (-wheelL.transform.InverseTransformPoint(hitL.point).y - wheelL.radius) / wheelL.suspensionDistance;

        bool groundedR = wheelR.GetGroundHit(out WheelHit hitR);
        if (groundedR)
            travelR = (-wheelR.transform.InverseTransformPoint(hitR.point).y - wheelR.radius) / wheelR.suspensionDistance;

        float antiRollForce = (travelL - travelR) * this.antiRollForce;

        if (groundedL)
            rb.AddForceAtPosition(wheelL.transform.up * -antiRollForce, wheelL.transform.position);
        if (groundedR)
            rb.AddForceAtPosition(wheelR.transform.up * antiRollForce, wheelR.transform.position);
    }

    private void LimitSpeed()
    {
        if (currentSpeed > maxSpeed)
        {
            Vector3 limitedVelocity = rb.linearVelocity.normalized * (maxSpeed / 3.6f);
            rb.linearVelocity = limitedVelocity;
        }
    }
    public float GetCurrentSpeed()
    {
        return isReversing ? -currentSpeed : currentSpeed;
    }

    public bool IsBraking()
    {
        return isBraking;
    }

    public bool IsReversing()
    {
        return isReversing;
    }
    public float MaxSpeeed
    {
        get { return maxSpeed; }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > collisionThreshold  && 
            !collision.gameObject.CompareTag("NoCollisionSound"))
        {
            CollisionSound.Play();
        }
    }
}