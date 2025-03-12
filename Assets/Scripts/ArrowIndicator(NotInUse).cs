using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicator : MonoBehaviour
{

    //NOT IN USE
    //NOT IN USE
    //NOT IN USE
    //NOT IN USE
    //NOT IN USE
    //NOT IN USE

    
    public Transform player;
    public Transform target;
    public RectTransform minimapRect;
    public float hideDistance = 10f;
    private RectTransform arrowRectTransform;
    private Image arrowImage;
    public float scale;   

    private void Start()
    {
        arrowRectTransform = GetComponent<RectTransform>();
        arrowImage = GetComponent<Image>();
        arrowImage.enabled = target != null;
    }

    private void Update()
    {
        if (target != null && player != null)
        {
            float distanceToTarget = Vector3.Distance(player.position, target.position);
            arrowImage.enabled = distanceToTarget > hideDistance;
            
            if (arrowImage.enabled)
            {
                UpdateArrowPositionAndDirection();
            }
        }
    }

    private void UpdateArrowPositionAndDirection()
    {
        // Get the positions in screen space
        Vector2 playerScreenPos = WorldToMinimapPoint(player.position);
        Vector2 targetScreenPos = WorldToMinimapPoint(target.position);

        // Calculate direction from player to target
        Vector2 direction = (targetScreenPos - playerScreenPos).normalized;
        
        // Calculate minimap bounds
        float minimapWidth = minimapRect.rect.width;
        float minimapHeight = minimapRect.rect.height;
        Vector2 minimapCenter = new Vector2(minimapWidth / 2, minimapHeight / 2);
        
        // Calculate intersection point with minimap edges
        Vector2 edgePosition = CalculateEdgeIntersection(playerScreenPos, direction, minimapWidth, minimapHeight);

        // Update arrow position
        arrowRectTransform.anchoredPosition = edgePosition;

        // Calculate and set arrow rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private Vector2 WorldToMinimapPoint(Vector3 worldPos)
    {
        // Convert world position to minimap position (assuming top-down view)
        // You might need to adjust this based on your game's scale and orientation
        float minimapScale = minimapRect.rect.width / scale; // Adjust 100f based on your world size
        Vector2 minimapCenter = new Vector2(minimapRect.rect.width / 2, minimapRect.rect.height / 2);
        
        return new Vector2(
            worldPos.x * minimapScale + minimapCenter.x,
            worldPos.z * minimapScale + minimapCenter.y
        );
    }

    private Vector2 CalculateEdgeIntersection(Vector2 origin, Vector2 direction, float width, float height)
    {
        float x = origin.x;
        float y = origin.y;
        Vector2 intersection = Vector2.zero;

        // Calculate time to hit vertical edges
        float txLeft = (0 - x) / direction.x;
        float txRight = (width - x) / direction.x;
        // Calculate time to hit horizontal edges
        float tyBottom = (0 - y) / direction.y;
        float tyTop = (height - y) / direction.y;

        // Find the closest positive intersection
        float[] times = { txLeft, txRight, tyBottom, tyTop };
        float minPositiveTime = float.MaxValue;
        
        for (int i = 0; i < times.Length; i++)
        {
            if (times[i] > 0 && times[i] < minPositiveTime)
            {
                minPositiveTime = times[i];
                intersection = origin + direction * minPositiveTime;
            }
        }

        // Clamp to ensure the arrow stays within bounds
        intersection.x = Mathf.Clamp(intersection.x, 0, width);
        intersection.y = Mathf.Clamp(intersection.y, 0, height);

        return intersection;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        arrowImage.enabled = target != null;
    }

    public void ChangeArrowColor(Color color)
    {
        arrowImage.color = color;
    }
}