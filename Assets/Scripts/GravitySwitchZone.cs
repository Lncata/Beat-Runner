using UnityEngine;

public class GravitySwitchZone : MonoBehaviour
{
    public bool invertedGravity = true;
    public bool destroyAfterUse = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovementInferno player = other.GetComponent<PlayerMovementInferno>();

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerMovementInferno>();
        }

        if (player == null) return;

        player.SetGravityInverted(invertedGravity);

        if (destroyAfterUse)
        {
            Destroy(gameObject);
        }
    }
}