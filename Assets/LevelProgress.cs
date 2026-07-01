using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelProgress : MonoBehaviour
{
    public Transform player;
    public Transform levelStart;
    public Transform levelEnd;
    public Slider progressSlider;
    public TMP_Text progressText;

    private float totalDistance;

    void Start()
    {
        totalDistance = levelEnd.position.x - levelStart.position.x;

        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1f;
        progressSlider.value = 0f;

        
    }

    void Update()
    {
        float currentDistance = player.position.x - levelStart.position.x;
        float progress = currentDistance / totalDistance;

        progress = Mathf.Clamp01(progress);

        progressSlider.value = progress;
        
    }
}