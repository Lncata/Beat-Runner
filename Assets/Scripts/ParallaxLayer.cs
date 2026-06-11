using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Transform cameraTransform;

    [Range(0f, 1f)]
    public float followX = 1f;

    [Range(0f, 1f)]
    public float followY = 1f;

    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        startPosition = transform.position;

        if (cameraTransform != null)
        {
            cameraStartPosition = cameraTransform.position;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 cameraDelta = cameraTransform.position - cameraStartPosition;

        transform.position = new Vector3(
            startPosition.x + cameraDelta.x * followX,
            startPosition.y + cameraDelta.y * followY,
            startPosition.z
        );
    }
}