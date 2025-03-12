using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    public Transform target;
    public Transform player;
    public SpriteRenderer arrowImage;
    public float hideDistance = 10f;
    public float offset = 90f;
    public float heightOffset = 2f;

    void Update()
    {
        if (target == null || player == null) return;

        transform.position = player.position + Vector3.up * heightOffset;

        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0;
        float distanceToTarget = directionToTarget.magnitude;
       
        arrowImage.enabled = distanceToTarget > hideDistance;

        if (arrowImage.enabled)
        {
            float targetAngle = Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg;
           
            transform.rotation = Quaternion.Euler(90, -targetAngle + offset, 0);

            Debug.DrawLine(transform.position, target.position, Color.red);
        }
    }

    public void ChangeArrowColor(Color color)
    {
        if (arrowImage != null)
        {
            arrowImage.color = color;
        }
    }
}