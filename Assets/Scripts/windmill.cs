using UnityEngine;

public class WindmillRotation : MonoBehaviour
{

    void Update()
{
    float speed = Mathf.Sin(Time.time) * 7f + 12f;
    transform.Rotate(Vector3.forward * speed * Time.deltaTime);
}

}
