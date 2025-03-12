using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public GameObject storeZones;
    public ArrowDirection arrowDirection;

    private void Start()
    {
        ActivateRandomPickupPoint();
    }

    public void ActivateRandomPickupPoint()
    {
        int childCount = storeZones.transform.childCount;
        if (childCount == 0) return;

        int randomIndex = Random.Range(0, childCount);
        GameObject randomPickupPoint = storeZones.transform.GetChild(randomIndex).gameObject;
        
        randomPickupPoint.SetActive(true);
        Debug.Log("Activated the Pickup Point number:"+ randomIndex);

        if (arrowDirection != null)
        {
                arrowDirection.target = storeZones.transform.GetChild(randomIndex);
                Debug.Log("Arrow is now pointing towards the zone");
                Color PickupColor = Color.red;
                arrowDirection.ChangeArrowColor(PickupColor);
        }
    }
}
