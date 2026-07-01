using UnityEngine;

public class CameraUpperZone : MonoBehaviour
{
    public CameraFollow_inferno cameraFollow;

    private int playerCollidersInside = 0;

    private void Start()
    {
        if (cameraFollow == null)
        {
            cameraFollow = Camera.main.GetComponent<CameraFollow_inferno>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerCollidersInside++;
            cameraFollow.SetUpperZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerCollidersInside--;

            if (playerCollidersInside <= 0)
            {
                playerCollidersInside = 0;
                cameraFollow.SetUpperZone(false);
            }
        }
    }
}