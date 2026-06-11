using UnityEngine;

public class CatTransformZone : MonoBehaviour
{
    public bool destroyAfterUse = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerFormSwitcher formSwitcher = other.GetComponent<PlayerFormSwitcher>();

        if (formSwitcher == null)
        {
            formSwitcher = other.GetComponentInParent<PlayerFormSwitcher>();
        }

        if (formSwitcher == null) return;

        formSwitcher.TransformToCat();

        if (destroyAfterUse)
        {
            Destroy(gameObject);
        }
    }
}