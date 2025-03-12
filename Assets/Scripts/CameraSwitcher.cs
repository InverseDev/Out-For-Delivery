using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;      // Assign the main camera (third-person view)
    public Camera fpsCamera;       // Assign the new FPS camera
    public bool isFPSView = false;  // To keep track of the current camera

    // NOT IN USE
    // NOT IN USE
    // NOT IN USE
    // NOT IN USE
    // NOT IN USE
    // NOT IN USE
    // NOT IN USE
    // NOT IN USE
    // NOT IN USE
    // NOT IN USE
    // NOT IN USE

    void Start()
    {
        // Ensure the main camera is active and FPS camera is inactive at the start
        mainCamera.gameObject.SetActive(true);
        fpsCamera.gameObject.SetActive(false);
    }


    public void SwitchCamera()
    {
        // Toggle the boolean flag
        isFPSView = !isFPSView;

        // Enable the FPS camera if the flag is true, otherwise enable the main camera
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
    }
}
