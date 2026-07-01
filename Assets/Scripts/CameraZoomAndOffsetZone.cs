using UnityEngine;

public class CameraZoomAndOffsetZone : MonoBehaviour
{
    [Header("Camara")]
    public Camera targetCamera;

    [Header("Zoom")]
    public float normalSize = 5f;
    public float zoomSize = 4f;
    public float zoomSpeed = 3f;

    [Header("Movimiento vertical de camara")]
    public float normalOffsetY = 0f;
    public float upperOffsetY = 1.5f;
    public float offsetSpeed = 3f;

    private float targetSize;
    private float targetOffsetY;
    private float currentOffsetY;

    private Transform cameraTransform;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        cameraTransform = targetCamera.transform;

        normalSize = targetCamera.orthographicSize;
        targetSize = normalSize;

        targetOffsetY = normalOffsetY;
        currentOffsetY = normalOffsetY;
    }

    private void LateUpdate()
    {
        targetCamera.orthographicSize = Mathf.Lerp(
            targetCamera.orthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );

        currentOffsetY = Mathf.Lerp(
            currentOffsetY,
            targetOffsetY,
            offsetSpeed * Time.deltaTime
        );

        cameraTransform.position += new Vector3(0f, currentOffsetY, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetSize = zoomSize;
            targetOffsetY = upperOffsetY;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetSize = normalSize;
            targetOffsetY = normalOffsetY;
        }
    }
}