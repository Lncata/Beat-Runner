using UnityEngine;

public class CameraFollow_inferno : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movimiento normal")]
    public float offsetX = 3f;

    public float smoothSpeedX = 5f;
    public float smoothSpeedY = 3f;

    public float verticalThreshold = 2f;
    public float verticalOffset = 1f;

    [Header("Zoom")]
    public Camera targetCamera;
    public float normalSize = 5f;
    public float upperZoneSize = 4f;
    public float zoomSpeed = 3f;

    [Header("Zona superior")]
    public float upperZoneExtraY = 0.5f;
    public float upperZoneSmooth = 3f;

    private float baseY;

    private bool inUpperZone;
    private float currentExtraY;
    private float targetExtraY;
    private float targetCameraSize;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (targetCamera != null)
        {
            normalSize = targetCamera.orthographicSize;
            targetCameraSize = normalSize;
        }

        if (target == null) return;

        float startY = transform.position.y;

        if (target.position.y > startY + verticalThreshold)
        {
            startY = target.position.y - verticalOffset;
        }

        transform.position = new Vector3(
            target.position.x + offsetX,
            startY,
            transform.position.z
        );

        baseY = startY;
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

        currentExtraY = Mathf.Lerp(
            currentExtraY,
            targetExtraY,
            upperZoneSmooth * Time.deltaTime
        );

        desiredY += currentExtraY;

        float newY = Mathf.Lerp(transform.position.y, desiredY, smoothSpeedY * Time.deltaTime);

        transform.position = new Vector3(newX, newY, transform.position.z);

        if (targetCamera != null)
        {
            targetCamera.orthographicSize = Mathf.Lerp(
                targetCamera.orthographicSize,
                targetCameraSize,
                zoomSpeed * Time.deltaTime
            );
        }
    }

    public void SetUpperZone(bool active)
    {
        inUpperZone = active;

        if (inUpperZone)
        {
            targetExtraY = upperZoneExtraY;
            targetCameraSize = upperZoneSize;
        }
        else
        {
            targetExtraY = 0f;
            targetCameraSize = normalSize;
        }
    }
}