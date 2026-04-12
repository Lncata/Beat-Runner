using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float offsetX = 3f;

    public float smoothSpeedX = 5f;
    public float smoothSpeedY = 3f;

    public float verticalThreshold = 2f;
    public float verticalOffset = 1f;

    private float baseY;

    void Start()
    {
        baseY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float desiredX = target.position.x + offsetX;
        float newX = Mathf.Lerp(transform.position.x, desiredX, smoothSpeedX * Time.deltaTime);

        float desiredY = baseY;

        if (target.position.y > baseY + verticalThreshold)
        {
            desiredY = target.position.y - verticalOffset;
        }

        float newY = Mathf.Lerp(transform.position.y, desiredY, smoothSpeedY * Time.deltaTime);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}