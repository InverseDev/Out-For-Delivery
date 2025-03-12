using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the NPCVehicleTurn component
        NPCVehicleTurn vehicleTurn = other.GetComponent<NPCVehicleTurn>();
        
        // If it does, trigger the turn behavior on the vehicle
        if (vehicleTurn != null)
        {
            vehicleTurn.TriggerTurn();
        }
    }
}
