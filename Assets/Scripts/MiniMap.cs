using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform player;
    public Transform MainCam;
    public CarController carController;
    private float shadowDistance;

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y; // Maintain the minimap height
        transform.position = newPosition;

        // Adjust rotation based on FPS view or main camera
        if (carController.isFPSView)
        {
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(90f, MainCam.eulerAngles.y, 0f);
        }
    }

    private void OnPreRender()
    {
        // Save the current shadow distance and set it to 0 before rendering the minimap
        shadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
        
    }

    private void OnPostRender()
    {
        // Restore the original shadow distance after rendering the minimap
        QualitySettings.shadowDistance = shadowDistance;
    }
}
