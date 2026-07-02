using UnityEngine;

/// <summary>
/// Capa puramente ADITIVA de "sensación rítmica": lee el AudioSource del nivel y da un pulso
/// visual (escala) a uno o varios transforms en cada beat, calculado desde el BPM.
/// No toca física, colisiones ni el movimiento del jugador: solo lee music.time y escribe la
/// localScale de los objetos indicados en <see cref="objetivos"/>.
///
/// Uso: poner este componente en cualquier GameObject de la escena, asignar el AudioSource que
/// reproduce la pista, el BPM de esa pista, y en "objetivos" arrastra transforms VISUALES
/// (p. ej. el sprite hijo del jugador y/o la cámara). Nunca usar el root del jugador con
/// Rigidbody2D/collider, porque escalar el collider alteraría las colisiones.
/// </summary>
public class BeatPulse : MonoBehaviour
{
    [Header("Fuente de ritmo")]
    [Tooltip("AudioSource que reproduce la pista del nivel (Play On Awake).")]
    public AudioSource music;
    [Tooltip("BPM real de la pista (Earth = 174).")]
    public float bpm = 174f;
    [Tooltip("Ajuste fino de fase en segundos si el pulso se ve adelantado/atrasado.")]
    public float offsetBeat = 0f;

    [Header("Pulso")]
    [Tooltip("Transforms visuales que laten con el beat. NO uses el root con collider.")]
    public Transform[] objetivos;
    [Tooltip("Cuánto crece la escala en el instante del beat.")]
    public float escalaPulso = 1.15f;
    [Tooltip("Qué tan rápido vuelve la escala a su valor base entre beats.")]
    public float velocidadDecaimiento = 8f;

    float secondsPerBeat;
    int lastBeat = -1;
    Vector3[] escalaBase;

    void Start()
    {
        secondsPerBeat = bpm > 0f ? 60f / bpm : 0.5f;

        // Guardamos la escala original de cada objetivo para volver siempre a ella y no acumular.
        if (objetivos != null)
        {
            escalaBase = new Vector3[objetivos.Length];
            for (int i = 0; i < objetivos.Length; i++)
            {
                escalaBase[i] = objetivos[i] != null ? objetivos[i].localScale : Vector3.one;
            }
        }
    }

    void Update()
    {
        // 1) Decaimiento continuo: cada objetivo vuelve suavemente a su escala base.
        if (objetivos != null)
        {
            for (int i = 0; i < objetivos.Length; i++)
            {
                if (objetivos[i] == null) continue;
                objetivos[i].localScale = Vector3.Lerp(
                    objetivos[i].localScale, escalaBase[i], velocidadDecaimiento * Time.deltaTime);
            }
        }

        // 2) Detección de beat (mismo patrón que MusicManager): solo LEE el audio.
        if (music == null || !music.isPlaying || secondsPerBeat <= 0f)
        {
            return;
        }

        int beat = Mathf.FloorToInt((music.time + offsetBeat) / secondsPerBeat);
        if (beat != lastBeat)
        {
            lastBeat = beat;
            DispararPulso();
        }
    }

    void DispararPulso()
    {
        if (objetivos == null) return;

        for (int i = 0; i < objetivos.Length; i++)
        {
            if (objetivos[i] == null) continue;
            objetivos[i].localScale = escalaBase[i] * escalaPulso;
        }
    }
}
