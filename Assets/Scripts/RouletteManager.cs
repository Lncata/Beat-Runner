using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class RouletteManager : MonoBehaviour
{
    public RectTransform wheel;

    private bool spinning = false;

    public BuffType currentBuff;

    private void Start()
    {
        Spin();
    }

    public void Spin()
    {
        if (!spinning)
        {
            StartCoroutine(SpinRoutine());
        }
    }

    IEnumerator SpinRoutine()
    {
        spinning = true;

        float duration = 8f;

        float elapsed = 0f;

        float startZ = wheel.eulerAngles.z;

        float targetZ = startZ - Random.Range(2000f, 4000f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            float curve = 1f - Mathf.Pow(1f - t, 3);

            float currentZ =
                Mathf.Lerp(startZ, targetZ, curve);

            wheel.rotation =
                Quaternion.Euler(0, 0, currentZ);

            yield return null;
        }

        wheel.rotation =
            Quaternion.Euler(0, 0, targetZ);

        DetermineReward();
       
        GameManager.Instance.selectedBuff = currentBuff;

        yield return new WaitForSeconds(2f);
        
        SceneManager.LoadScene("level_Earth");

        spinning = false;
    }

    void DetermineReward()
    {
        float finalAngle =
            wheel.eulerAngles.z;

        finalAngle = (360 - finalAngle) % 360;

        Debug.Log("Ángulo final: " + finalAngle);

        if(finalAngle >= 0 && finalAngle < 72)
        {
            currentBuff = BuffType.ExtraLife;
        }
        else if(finalAngle >= 72 && finalAngle < 144)
        {
            currentBuff = BuffType.ScoreMultiplier;
        }
        else if(finalAngle >= 144 && finalAngle < 216)
        {
            currentBuff = BuffType.SpeedMultiplier;
        }
        else if(finalAngle >= 216 && finalAngle < 288)
        {
            currentBuff = BuffType.MiniRunner;
        }
        else
        {
            currentBuff = BuffType.None;
        }

        Debug.Log("BUFF OBTENIDO: " + currentBuff);
    }
}