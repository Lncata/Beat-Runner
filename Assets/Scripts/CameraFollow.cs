using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // el player
    public float offsetX = 2f; // distancia adelante
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offsetX,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}