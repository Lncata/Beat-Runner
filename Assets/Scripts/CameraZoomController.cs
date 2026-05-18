using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    public Camera cam;

    public float zoomSpeed = 5f;

    private float normalSize;
    private float targetSize;

    void Awake()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        if (cam != null)
        {
            normalSize = cam.orthographicSize;
            targetSize = normalSize;
        }
    }

    void Update()
    {
        if (cam == null) return;

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );
    }

    public void SetZoom(float newSize)
    {
        targetSize = newSize;
    }

    public void ResetZoom()
    {
        targetSize = normalSize;
    }
}