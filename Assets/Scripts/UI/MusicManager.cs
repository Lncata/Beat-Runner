using System;
using UnityEngine;

/// <summary>
/// (Opcional) Reproduce la pista del nivel y expone un reloj de beats para el pulso del HUD.
/// Carga el clip desde Resources; si no lo encuentra, corre en silencio (el HUD funciona igual).
/// Es la ÚNICA dependencia de asset de esta capa, y es de audio — no toca arte ni la escena visual.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Ruta dentro de cualquier carpeta Resources/, sin extensión.")]
    public string resourcePath = "Music/level_tierra";  // ← ajusta a tu pista
    public float bpm = 174f;                             // ← BPM de tu pista
    [Range(0f, 1f)]
    public float volume = 0.7f;

    public float SongTime => source != null && source.clip != null ? source.time : 0f;
    public int CurrentBeat { get; private set; }
    public event Action<int> OnBeat;

    AudioSource source;
    float secondsPerBeat;
    int lastBeat = -1;

    public void Initialize()
    {
        secondsPerBeat = bpm > 0f ? 60f / bpm : 0.5f;

        source = GetComponent<AudioSource>();
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }
        source.playOnAwake = false;
        // Loop: la pista suele ser más corta que el nivel; al reiniciar, el contador de beats
        // vuelve a 0 pero OnBeat se sigue disparando (su único consumidor es el pulso del HUD).
        source.loop = true;
        source.volume = volume;

        AudioClip clip = Resources.Load<AudioClip>(resourcePath);
        if (clip != null)
        {
            source.clip = clip;
        }
        else
        {
            Debug.LogWarning($"[MusicManager] No se encontró el clip en Resources/{resourcePath}. " +
                             "El nivel correrá sin música (el HUD funciona igual).");
        }
    }

    public void Play()
    {
        if (source != null && source.clip != null)
        {
            lastBeat = -1;
            source.Play();
        }
    }

    public void Stop()
    {
        if (source != null)
        {
            source.Stop();
        }
    }

    public void Pause()
    {
        if (source != null)
        {
            source.Pause();
        }
    }

    public void Resume()
    {
        if (source != null)
        {
            source.UnPause();
        }
    }

    void Update()
    {
        if (source == null || !source.isPlaying || secondsPerBeat <= 0f)
        {
            return;
        }

        int beat = Mathf.FloorToInt(source.time / secondsPerBeat);
        if (beat != lastBeat)
        {
            lastBeat = beat;
            CurrentBeat = beat;
            OnBeat?.Invoke(beat);
        }
    }
}
