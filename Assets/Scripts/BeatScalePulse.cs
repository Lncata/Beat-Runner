using UnityEngine;

/// <summary>
/// Adjuntado al objeto "obstacles" por CieloBeatManager.
/// En cada beat escala brevemente todos los hijos (spikes) para dar feedback visual al ritmo.
/// </summary>
public class BeatScalePulse : MonoBehaviour
{
    [Tooltip("Cuánto crece el spike en el beat (1.0 = sin cambio)")]
    public float pulseScale = 1.2f;

    [Tooltip("Duración del pulso en segundos")]
    public float pulseDuration = 0.08f;

    Transform[] children;
    Vector3[] baseScales;
    float pulseTimer;
    bool pulsing;

    void Start()
    {
        int count = transform.childCount;
        children = new Transform[count];
        baseScales = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            children[i] = transform.GetChild(i);
            baseScales[i] = children[i].localScale;
        }

        if (CieloBeatManager.Instance != null)
            CieloBeatManager.Instance.OnBeat += Pulse;
        else
            Debug.LogWarning("[BeatScalePulse] CieloBeatManager no encontrado al iniciar.");
    }

    void OnDestroy()
    {
        if (CieloBeatManager.Instance != null)
            CieloBeatManager.Instance.OnBeat -= Pulse;
    }

    void Pulse()
    {
        pulsing = true;
        pulseTimer = pulseDuration;

        for (int i = 0; i < children.Length; i++)
            if (children[i] != null)
                children[i].localScale = baseScales[i] * pulseScale;
    }

    void Update()
    {
        if (!pulsing) return;

        pulseTimer -= Time.deltaTime;
        if (pulseTimer <= 0f)
        {
            pulsing = false;
            for (int i = 0; i < children.Length; i++)
                if (children[i] != null)
                    children[i].localScale = baseScales[i];
        }
    }
}
