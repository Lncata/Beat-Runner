using UnityEngine;

public class NormalTransformZone : MonoBehaviour
{
    public bool destroyAfterUse = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Buscamos el script de transformación en el jugador
        PlayerFormSwitcher formSwitcher = other.GetComponent<PlayerFormSwitcher>();

        if (formSwitcher == null)
        {
            formSwitcher = other.GetComponentInParent<PlayerFormSwitcher>();
        }

        if (formSwitcher == null) return;

        formSwitcher.TransformToNormal();

        if (destroyAfterUse)
        {
            Destroy(gameObject);
        }
    }
}