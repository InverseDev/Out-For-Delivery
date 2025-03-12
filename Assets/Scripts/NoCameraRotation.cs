using UnityEngine;

public class CameraConstraint : MonoBehaviour
{
    public float minXRotation = -30f; // Minimum X rotation angle
    public float maxXRotation = 60f;  // Maximum X rotation angle

    private void LateUpdate()
    {
        // Get the current rotation
        Vector3 currentRotation = transform.localEulerAngles;

        // Clamp the X rotation
        float clampedXRotation = ClampAngle(currentRotation.x, minXRotation, maxXRotation);

        // Apply the clamped rotation
        transform.localEulerAngles = new Vector3(clampedXRotation, currentRotation.y, currentRotation.z);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + min);
        return Mathf.Min(angle, max);
    }
}