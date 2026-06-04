using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;

    public int multiplier = 1;

    public TMP_Text scoreText;

    private float timer;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance.selectedBuff
            == BuffType.ScoreMultiplier)
        {
            multiplier = 2;

            Debug.Log("BUFF ACTIVADO: SCORE x2");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 1f)
        {
            score += 10 * multiplier;

            scoreText.text =
                "Score: " + score;

            timer = 0f;
        }
    }
}