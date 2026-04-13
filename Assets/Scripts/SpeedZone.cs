using UnityEngine;

public class SpeedZone : MonoBehaviour
{
    public float newPlayerSpeed = 8f;
    public float newCameraSmoothX = 8f;
    public float newCameraOffsetX = 4f;

    public bool destroyAfterUse = true;

    private CameraFollow cameraFollow;

    void Start()
    {
        if (Camera.main != null)
        {
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Algo entró al trigger: " + other.name);

        PlayerMovementInferno player = other.GetComponent<PlayerMovementInferno>();

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerMovementInferno>();
        }

        if (player == null)
        {
            Debug.Log("No encontré PlayerMovement");
            return;
        }

        Debug.Log("SpeedZone activada");

        player.speed = newPlayerSpeed;

        if (cameraFollow != null)
        {
            cameraFollow.smoothSpeedX = newCameraSmoothX;
            cameraFollow.offsetX = newCameraOffsetX;
        }

        if (destroyAfterUse)
        {
            Destroy(gameObject);
        }
    }
}