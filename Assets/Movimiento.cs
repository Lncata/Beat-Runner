using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Movimiento : MonoBehaviour
{
    private Rigidbody2D rb2D;

    [Header("Movimiento")]
    private float movimientoHorizontal = 5f;
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float suavizadoMovimiento;
    private Vector2 velocidad = Vector2.zero;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogError("Movimiento: no se encontró Rigidbody2D en el GameObject. Añade un Rigidbody2D para que el movimiento funcione.");
        }
    }

    private void Update()
    {
        // Movimiento horizontal perpetuo hacia la derecha (positivo)
        // Usamos el valor absoluto de velocidadMovimiento para asegurar que sea positivo
        movimientoHorizontal = Mathf.Abs(velocidadMovimiento);
    }

    private void FixedUpdate()
    {
        // Pasamos la velocidad objetivo directamente (no multiplicar por deltaTime cuando seteamos velocity)
        Mover(movimientoHorizontal);
    }

    private void Mover(float mover)
    {
        if (rb2D == null) return;

        // Mantener la componente Y actual (gravedad, salto, etc.) y suavizar solo la X
        Vector2 velocidadObjetivo = new Vector2(mover, rb2D.linearVelocity.y);
        rb2D.linearVelocity = Vector2.SmoothDamp(rb2D.linearVelocity, velocidadObjetivo, ref velocidad, suavizadoMovimiento);
    }
}
