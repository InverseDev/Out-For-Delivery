using UnityEngine;

public class MinimapCamera : MonoBehaviour 
{
    void Start()
    {
        Camera minimapCamera = GetComponent<Camera>();
        minimapCamera.clearFlags = CameraClearFlags.SolidColor;
        
        // This will make the camera not render any lights
        minimapCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Lights"));
    }
}