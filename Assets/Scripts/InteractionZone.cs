using UnityEngine;

public class ZoneInteractionManager : MonoBehaviour
{
    public float interactionTime = 2f;
    public LevelManager levelManager;
    private bool playerInZone = false;
    private float timer = 0f;

    [Header("Zone Type")]
    public bool isPickupZone;

    [Header("References")]
    public GameObject deliveryZonesParent;
    public GameObject storeZonesParent;
    public ArrowDirection arrowDirection;
    public PickupManager pickupManager;

    [Header("Particle System Prefab")]
    public GameObject particleSystemPrefab; // Assign the prefab in the inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCar"))
        {
            playerInZone = true;
            timer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ResetZoneInteraction();
    }

    private void Update()
    {
        if (playerInZone)
        {
            timer += Time.deltaTime;

            if (timer >= interactionTime)
            {
                if (isPickupZone)
                {
                    Debug.Log("Pickup Complete! Activating random delivery zone...");
                    ActivateRandomDeliveryZone();
                }
                else
                {
                    levelManager.CompleteLevel(); 
                }

                ResetZoneInteraction();
            }
        }
    }

    private void ActivateRandomDeliveryZone()
    {
        if (deliveryZonesParent != null)
        {
            int childCount = deliveryZonesParent.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                deliveryZonesParent.transform.GetChild(i).gameObject.SetActive(false);
            }

            int randomIndex = Random.Range(0, childCount);
            Transform selectedDeliveryZone = deliveryZonesParent.transform.GetChild(randomIndex);
            selectedDeliveryZone.gameObject.SetActive(true);
            Debug.Log($"Delivery zone {randomIndex + 1} activated.");

            // Add the particle system prefab as a child to the selected delivery zone
            if (particleSystemPrefab != null)
            {
                GameObject particleInstance = Instantiate(particleSystemPrefab, selectedDeliveryZone);
                particleInstance.transform.localPosition = Vector3.zero; // Optional: Adjust position relative to the zone
                Debug.Log("Particle system added to the activated delivery zone.");
            }
            else
            {
                Debug.LogWarning("Particle system prefab is not assigned!");
            }

            if (arrowDirection != null)
            {
                arrowDirection.target = selectedDeliveryZone;
                Debug.Log("Arrow is now pointing towards the zone");
                Color32 DeliveryColor = new Color32(70, 197, 12, 255);
                arrowDirection.ChangeArrowColor(DeliveryColor);
            }
        }
        else
        {
            Debug.LogWarning("Delivery zones parent not assigned!");
        }

        gameObject.SetActive(false);
    }

    private void ResetZoneInteraction()
    {
        playerInZone = false;
        timer = 0f;
    }
}
