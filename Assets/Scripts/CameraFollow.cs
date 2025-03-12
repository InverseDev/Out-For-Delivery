using UnityEngine;
using UnityEngine.EventSystems;

public class CameraFollowTouch : MonoBehaviour
{
    public Transform car;
    public Vector3 offset = new Vector3(0, 5, -10);
    private Vector3 defaultOffset;
    public float followSpeed = 5f;
    public float rotationSpeed = 0.2f;
    public float touchDeadzone = 10f;

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private Vector2 previousTouchPosition;
    private int cameraControlFingerId = -1;
    private Vector3 velocity = Vector3.zero;

    private const float SmoothTime = 0.15f;
    private const float MaxRotationXAngle = 80f;
    private const float MinRotationXAngle = -20f;

    private float defaultRotationX;
    private float defaultRotationY;
    private float lastInteractionTime;
    public float resetDelay = 5f;
    
    public MiniMapOpen minimapController;

    private void Start()
    {
        if (car == null)
        {
            enabled = false;
            return;
        }

        // Store initial rotation and offset
        defaultOffset = offset;
        Vector3 initialRotation = transform.eulerAngles;
        defaultRotationX = currentRotationX = initialRotation.x;
        defaultRotationY = currentRotationY = initialRotation.y;
        lastInteractionTime = Time.time;
    }

    private void LateUpdate()
    {
        FollowCar();
        
        if (minimapController != null && minimapController.IsDraggingMap())
        {
            return;
        }
        
        HandleTouchRotation();
        CheckForReset();
    }

    private void FollowCar()
    {
        Quaternion targetRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        Vector3 targetPosition = car.position + targetRotation * offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        transform.LookAt(car);
    }

    private void HandleTouchRotation()
    {
        if (Input.touchCount == 0)
        {
            cameraControlFingerId = -1;
            return;
        }

        foreach (Touch touch in Input.touches)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                continue;

            if (touch.phase == TouchPhase.Began)
            {
                if (minimapController != null && minimapController.IsDraggingMap())
                {
                    return;
                }

                if (cameraControlFingerId == -1)
                {
                    cameraControlFingerId = touch.fingerId;
                    previousTouchPosition = touch.position;
                }
            }

            if (cameraControlFingerId == touch.fingerId)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Moved:
                        Vector2 touchDelta = touch.position - previousTouchPosition;
                        float targetRotationY = currentRotationY + touchDelta.x * rotationSpeed;
                        float targetRotationX = currentRotationX - touchDelta.y * rotationSpeed;
                        currentRotationY = Mathf.Lerp(currentRotationY, targetRotationY, 0.2f);
                        currentRotationX = Mathf.Clamp(Mathf.Lerp(currentRotationX, targetRotationX, 0.2f), MinRotationXAngle, MaxRotationXAngle);
                        previousTouchPosition = touch.position;
                        lastInteractionTime = Time.time;
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == cameraControlFingerId)
                        {
                            cameraControlFingerId = -1;
                        }
                        break;
                }
            }
        }
    }

   private void CheckForReset()
    {
        if (Time.time - lastInteractionTime > resetDelay)
        {
            currentRotationX = Mathf.Lerp(currentRotationX, defaultRotationX, 0.05f);

            float targetRotationY = car.eulerAngles.y;
            float angleDifference = Mathf.DeltaAngle(currentRotationY, targetRotationY);
            currentRotationY += angleDifference * 0.05f; 

            offset = Vector3.Lerp(offset, defaultOffset, 0.05f);
        }
    }

}
