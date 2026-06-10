using UnityEngine;

public class TestStartPosition : MonoBehaviour
{
    public Transform testSpawn;

    void Awake()
    {
        if (testSpawn != null)
        {
            transform.position = testSpawn.position;
        }
    }
}